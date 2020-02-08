﻿/*
 * Copyright (c) 2018 Google Inc.
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

using CommandLine;
using System;
using System.IO;
using Google.Cloud.TextToSpeech.V1;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices.ComTypes;

namespace GoogleCloudSamples
{
    [Verb("list", HelpText = "List available voices.")]
    class ListArgs
    {
        [Option('l', HelpText = "Desired language for the voice")]
        public string DesiredLanguage { get; set; }
    }

    class BaseTextToSpeechOptions
    {
        [Option('f', HelpText = "Source formatting")]
        public string SourceFormat { get; set; }
    }

    [Verb("synthesize", HelpText = "Synthesize input to audio")]
    class SynthesizeArgs : BaseTextToSpeechOptions
    {
        [Value(0, HelpText = "The text to synthesize.",
            Required = true)]
        public string Text { get; set; }
    }

    [Verb("synthesize-file", HelpText = "Synthesize a file to audio")]
    class SynthesizeFileArgs : BaseTextToSpeechOptions
    {
        [Value(0, HelpText = "The file to synthesize",
            Required = true)]
        public string Text { get; set; }
    }

    [Verb("synthesize-file-line", HelpText = "Synthesize a file to audio line by line")]
    class SynthesizeFileLineArgs : BaseTextToSpeechOptions
    {
        [Value(0, HelpText = "The file to synthesize", Required = true)]
        public string FileName { get; set; }

        [Option('n', HelpText = "Num instances", Default = 1)]
        public int NumInstances { get; set; }

        [Option('l', HelpText = "Total lines to be synthesized", Default = 10)]
        public int Lines { get; set; }

        [Option('g', HelpText = "To use new guid or use text in file", Default = false)]
        public bool NewGuid { get; set; }
    }


    [Verb("synthesize-with-profile",
          HelpText = "Synthesize text and optimize output for a device profile")]
    class SynthesizeWithEffectsArgs : SynthesizeFileArgs
    {
        [Option('o', HelpText = "Output file name", Required = true)]
        public string OutputFileName { get; set; }

        [Option('e', HelpText = "Effects profile to use", Required = true)]
        public string EffectsProfileId { get; set; }
    }

    public class TextToSpeech
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Parser.Default.ParseArguments<ListArgs, SynthesizeArgs,
                SynthesizeFileArgs, SynthesizeWithEffectsArgs, SynthesizeFileLineArgs>(args).MapResult(
                (ListArgs largs) => largs.DesiredLanguage == null ?
                    ListVoices() : ListVoices(largs.DesiredLanguage),
                (SynthesizeWithEffectsArgs sargs) =>
                    SynthesizeTextWithAudioProfile(sargs.Text,
                                                   sargs.OutputFileName,
                                                   sargs.EffectsProfileId),
                (SynthesizeArgs sargs) => Synthesize(sargs),
                (SynthesizeFileArgs sfargs) => SynthesizeFile(sfargs),
                (SynthesizeFileLineArgs sfargs) => SynthesizeFileLineText(sfargs),
                errs => 1);
        }

        // [START tts_list_voices]
        /// <summary>
        /// Lists all the voices available for speech synthesis.
        /// </summary>
        /// <param name="desiredLanguageCode">Language code to filter on</param>
        public static int ListVoices(string desiredLanguageCode = "")
        {
            TextToSpeechClient client = TextToSpeechClient.Create();

            // Performs the list voices request
            var response = client.ListVoices(new ListVoicesRequest
            {
                LanguageCode = desiredLanguageCode
            });

            foreach (Voice voice in response.Voices)
            {
                // Display the voices's name.
                Console.WriteLine($"Name: {voice.Name}");

                // Display the supported language codes for this voice.
                foreach (var languageCode in voice.LanguageCodes)
                {
                    Console.WriteLine($"Supported language(s): {languageCode}");
                }

                // Display the SSML Voice Gender
                Console.WriteLine("SSML Voice Gender: " +
                    (SsmlVoiceGender)voice.SsmlGender);

                // Display the natural sample rate hertz for this voice.
                Console.WriteLine("Natural Sample Rate Hertz: " +
                    voice.NaturalSampleRateHertz);
            }
            return 0;
        }
        // [END tts_list_voices]


        private static int Synthesize(SynthesizeArgs args)
        {
            if (args.SourceFormat != null && args.SourceFormat.Equals("ssml"))
            {
                SynthesizeSSML(args.Text);
            }
            else
            {
                SynthesizeText(args.Text);
            }
            return 0;
        }

        // [START tts_synthesize_text]
        /// <summary>
        /// Creates an audio file from the text input.
        /// </summary>
        /// <param name="text">Text to synthesize into audio</param>
        /// <remarks>
        /// Generates a file named 'output.mp3' in project folder.
        /// </remarks>
        public static void SynthesizeText(string text)
        {
            TextToSpeechClient client = TextToSpeechClient.Create();
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = new SynthesisInput
                {
                    Text = text
                },
                // Note: voices can also be specified by name
                Voice = new VoiceSelectionParams
                {
                    LanguageCode = "en-US",
                    SsmlGender = SsmlVoiceGender.Female
                },
                AudioConfig = new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3
                }
            });

            using (Stream output = File.Create("output.mp3"))
            {
                response.AudioContent.WriteTo(output);
            }
        }
        // [END tts_synthesize_text]

        // [START tts_synthesize_ssml]
        /// <summary>
        /// Creates an audio file from the SSML-formatted string.
        /// </summary>
        /// <param name="ssml">SSML string to synthesize</param>
        /// <remarks>
        /// Generates a file named 'output.mp3' in project folder.
        /// Note: SSML must be well-formed according to:
        ///    https://www.w3.org/TR/speech-synthesis/
        /// </remarks>
        public static void SynthesizeSSML(string ssml)
        {
            var client = TextToSpeechClient.Create();
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = new SynthesisInput
                {
                    Ssml = ssml
                },
                // Note: voices can also be specified by name
                Voice = new VoiceSelectionParams
                {
                    LanguageCode = "en-US",
                    SsmlGender = SsmlVoiceGender.Female
                },
                AudioConfig = new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3
                }
            });

            using (Stream output = File.Create("output.mp3"))
            {
                response.AudioContent.WriteTo(output);
            }
        }
        // [END tts_synthesize_ssml]

        /// <summary>
        ///  get one line from buffer to synthesize
        /// </summary>
        /// <returns>null if empty buffer, or return the sentence</returns>
        public static string GetOneSentence()
        {
            lock (linelist)
            {
                if (linelist.Count > 0)
                {
                    string line = linelist[0];
                    linelist.RemoveAt(0);
                    return line;
                }
                return null;
            }
        }

        public static List<string> linelist = new List<string>();
        /// <summary>
        /// Synthesize file line by line, only for text
        /// </summary>
        /// <param name="args">input args like total lines, num of instances</param>
        internal static int SynthesizeFileLineText(SynthesizeFileLineArgs args)
        {
            string[] lines = File.ReadAllLines(args.FileName);
            for (int i = 0; i < args.Lines; i++)
            {
                if (args.NewGuid)
                {
                    linelist.Add(Guid.NewGuid().ToString());
                }
                else
                {
                    linelist.Add(lines[i % lines.Length]);
                }
            }
            Console.WriteLine($"{linelist.Count} lines found");

            List<Task> taskList = new List<Task>();
            string testid = Guid.NewGuid().ToString();
            Console.WriteLine($"Start at [{DateTime.Now}] with run id {testid}");
            for (int i = 0; i < args.NumInstances; i++)
            {
                taskList.Add(Task.Run(new Action(new SynthesizeThread(testid).Synthesize)));
            }
            Task.WaitAll(taskList.ToArray());

            Console.WriteLine($"Avg FBL: {totalFirstSeconds / lineCount}, lines {lineCount}, rtf {totalFirstSeconds / totalAudioSeconds}");
            Console.WriteLine("Done");
            return 0;
        }

        static double totalFirstSeconds = 0;
        static double totalAudioSeconds = 0;
        static int lineCount = 0;
        internal static void SumUp(int kount, double ts, double ats)
        {
            lineCount += kount;
            totalFirstSeconds += ts;
            totalAudioSeconds += ats;
        }

        private static int SynthesizeFile(SynthesizeFileArgs args)
        {
            if (args.SourceFormat != null && args.SourceFormat.Equals("ssml"))
            {
                SynthesizeSSMLFile(args.Text);
            }
            else
            {
                SynthesizeTextFile(args.Text);
            }
            return 0;
        }

        // [START tts_synthesize_text_file]
        /// <summary>
        /// Creates an audio file from the input file of text.
        /// </summary>
        /// <param name="textFilePath">Path to text file</param>
        /// <remarks>
        /// Generates a file named 'output.mp3' in project folder.
        /// </remarks>
        public static void SynthesizeTextFile(string textFilePath)
        {
            string text = File.ReadAllText(textFilePath);

            var client = TextToSpeechClient.Create();
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = new SynthesisInput
                {
                    Text = text
                },
                // Note: voices can also be specified by name
                Voice = new VoiceSelectionParams
                {
                    LanguageCode = "en-US",
                    SsmlGender = SsmlVoiceGender.Female
                },
                AudioConfig = new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3
                }
            });

            using (Stream output = File.Create("output.mp3"))
            {
                response.AudioContent.WriteTo(output);
            }
        }
        // [START tts_synthesize_text_file]

        // [START tts_synthesize_ssml_file]
        /// <summary>
        /// Creates an audio file from the input SSML file.
        /// </summary>
        /// <param name="ssmlFilePath">Path to SSML file</param>
        /// <remarks>
        /// Generates a file named 'output.mp3' in project folder.
        /// Note: SSML must be well-formed according to:
        ///    https://www.w3.org/TR/speech-synthesis/
        /// </remarks>
        public static void SynthesizeSSMLFile(string ssmlFilePath)
        {
            string text = File.ReadAllText(ssmlFilePath);

            var client = TextToSpeechClient.Create();
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = new SynthesisInput
                {
                    Ssml = text
                },
                // Note: voices can also be specified by name
                Voice = new VoiceSelectionParams
                {
                    LanguageCode = "en-US",
                    SsmlGender = SsmlVoiceGender.Female
                },
                AudioConfig = new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3
                }
            });

            using (Stream output = File.Create("output.mp3"))
            {
                response.AudioContent.WriteTo(output);
            }
        }
        // [START tts_synthesize_ssml_file]

        // [START tts_synthesize_text_audio_profile]
        /// <summary>
        /// Creates an audio file from the text input, applying the specifed
        /// device profile to the output.
        /// </summary>
        /// <param name="text">Text to synthesize into audio</param>
        /// <param name="outputFile">Name of audio output file</param>
        /// <param name="effectProfileId">Audio effect profile to apply</param>
        /// <remarks>
        /// Output file saved in project folder.
        /// </remarks>
        public static int SynthesizeTextWithAudioProfile(string text,
                                                         string outputFile,
                                                         string effectProfileId)
        {
            var client = TextToSpeechClient.Create();
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = new SynthesisInput
                {
                    Text = text
                },
                // Note: voices can also be specified by name
                // Names of voices can be retrieved with client.ListVoices().
                Voice = new VoiceSelectionParams
                {
                    LanguageCode = "en-US",
                    SsmlGender = SsmlVoiceGender.Female
                },
                AudioConfig = new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3,
                    // Note: you can pass in multiple audio effects profiles.
                    // They are applied in the same order as provided.
                    EffectsProfileId = { effectProfileId }
                }
            });

            // The response's AudioContent is binary.
            using (Stream output = File.Create(outputFile))
            {
                response.AudioContent.WriteTo(output);
            }

            return 0;
        }
        // [END tts_synthesize_text_audio_profile]
    }
}
