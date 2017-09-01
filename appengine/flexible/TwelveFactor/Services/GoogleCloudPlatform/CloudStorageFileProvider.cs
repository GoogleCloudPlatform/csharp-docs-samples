/*
 * Copyright (c) 2017 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Google.Apis.Auth.OAuth2;
using Google.Api.Gax;
using Google.Apis.CloudRuntimeConfig.v1beta1;
using Google.Apis.Services;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TwelveFactor.Services.GoogleCloudPlatform {

    /// <summary>
    /// An implementation of IFileProvider that gets files from
    /// Google Cloud Storage.
    /// </summary>
    class CloudStorageFileProvider : IFileProvider
    {        

        // Configuration Sources are loaded before logging is configured,
        // because logging is controlled by configuration.  Therefore,
        // we have to queue all our log messages and then log them later.
        readonly DelayedLogger _logger = new DelayedLogger();
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger.InnerLogger = value; }
        }        

        readonly StorageClient _storage = StorageClient.Create();
        readonly TimeSpan? _pollingInterval;
     
        /// <param name="pollingInterval">How often Watched() files should
        /// be polled to detect changes.  A null value means Watch()
        /// always returns null.</param>
        public CloudStorageFileProvider(TimeSpan? pollingInterval = null)
        {
            _pollingInterval = pollingInterval;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new NotImplementedException();            
        }
        
        public IFileInfo GetFileInfo(string subpath)
        {
            var path = SplitObjectPath(subpath);
            try 
            {
                var obj = GetObject(path);
                if (obj != null) {
                    return new CloudStorageFileInfo(obj, _storage, _logger);                    
                }
            }
            catch (Exception e)
            {
                _logger.LogError(0, e, "GetFileInfo({0}) failed.", subpath);
            }
            return new NotFoundFileInfo(subpath);
        }

        struct ObjectPath {
            public string BucketName;
            public string ObjectName;
        }

        ObjectPath SplitObjectPath(string path)
        {
            // Accept paths in a variety of forms:
            //   bucket/a/b/c
            //   /bucket/a/b/c
            //   gs://bucket/a/b/c
            string gsPrefix = "gs:/";
            if (path.ToLower().Substring(0, gsPrefix.Length) == gsPrefix) {
                path = path.Substring(gsPrefix.Length);
            }
            if ('/' == path.FirstOrDefault()) {
                path = path.Substring(1);
            }
            string[] fragments = path.Split(new [] {'/'}, 2);
            return new ObjectPath()
            {
                BucketName = fragments.FirstOrDefault(),
                ObjectName = fragments.LastOrDefault()
            };
        }

        Google.Apis.Storage.v1.Data.Object GetObject(ObjectPath path) {
            try 
            {
                return _storage.GetObject(path.BucketName, 
                    path.ObjectName);
            }
            catch (Google.GoogleApiException e)
            when (e.HttpStatusCode == HttpStatusCode.NotFound) 
            {
                _logger.LogWarning("{0}/{1} not found.", path.BucketName,
                    path.ObjectName);
                return null;
            }
        }

        public IChangeToken Watch(string filter)
        {
            if (_pollingInterval == null) {
                return null;
            }
            var path = SplitObjectPath(filter);
            var token = new CloudStorageChangeToken();
            long? generation = null;
            var obj = GetObject(path);
            if (obj != null) {
                generation = obj.Generation;            
            }
            Task.Run(async () => {
                while (true) {
                    await Task.Delay(_pollingInterval.Value);
                    try
                    {
                        obj = GetObject(path);
                        if (generation != obj?.Generation) {
                            token.HasChanged = true;
                            return;
                        }
                    } 
                    catch (Exception e) 
                    {
                        _logger.LogError(0, e, "Exception while watching {0}",
                            filter);
                    }
                }
            });
            return token;            
        }            
    }

    class CloudStorageChangeToken : IChangeToken
    {
        List<ChangeCallback> _callbacks = new List<ChangeCallback>();
        bool _hasChanged = false;

        public bool HasChanged { 
            get { return _hasChanged; }
            set {
                _hasChanged = true;
                foreach (var callback in _callbacks) {
                    callback.Invoke();
                }
            }
        }

        public bool ActiveChangeCallbacks => true;

        public IDisposable RegisterChangeCallback(Action<object> callback,
            object state)
        {
            var disposable = new ChangeCallback() {
                Callback = callback,
                State = state
            };
            _callbacks.Add(disposable);
            return disposable;
        }

                class ChangeCallback : IDisposable {
            public Action<object> Callback { get; set; }
            public object State {get; set;}

            private object _lock = new object();
            private bool _done = false;
            public void Dispose()
            {
                lock(_lock) {
                    if (!_done) {
                        _done = true;
                    }
                }
            }

            public void Invoke() {
                lock(_lock) {
                    if (!_done) {
                        _done = true;
                        Callback(State);
                    }
                }                
            }
        };
    }

    class CloudStorageFileInfo : IFileInfo
    {
        readonly Google.Apis.Storage.v1.Data.Object _cloudStorageObject;
        readonly StorageClient _storage;
        readonly ILogger _logger;

        public CloudStorageFileInfo(
            Google.Apis.Storage.v1.Data.Object cloudStorageObject,
            StorageClient storageClient, ILogger logger)
        {
            _cloudStorageObject = cloudStorageObject;
            _storage = storageClient;
            _logger = logger;
        }

        public bool Exists => true;

        public long Length => (long) _cloudStorageObject.Size.Value;

        public string PhysicalPath => null;

        public string Name => _cloudStorageObject.Name;

        public DateTimeOffset LastModified => 
            _cloudStorageObject.Updated.Value;

        public bool IsDirectory => false;

        public Stream CreateReadStream()
        {
            // Json config files are small, and will easily fit in memory.
            var stream = new MemoryStream();
            _storage.DownloadObject(_cloudStorageObject.Bucket, 
                _cloudStorageObject.Name, stream);
            _logger.LogInformation("Loaded {0}/{1}", _cloudStorageObject.Bucket,
                _cloudStorageObject.Name);
            _logger.LogDebug("Body:\n{0}",
                System.Text.Encoding.UTF8.GetString(stream.ToArray()));
            return stream;
        }
    }
}
