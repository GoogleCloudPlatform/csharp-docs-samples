/*
 * Copyright 2020 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
 
using Google.Cloud.ServiceDirectory.V1;
using Xunit;

[Collection(nameof(ServiceDirectoryFixture))]
public class CreateNamespaceTest
{ 
    private readonly ServiceDirectoryFixture _fixture;

    public CreateNamespaceTest(ServiceDirectoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void CreatesNamespace()
    {
        var namespaceId = _fixture.RandomResourceId;
        _fixture.TempNamespaceIds.Add(namespaceId);
        // Run the sample code.
        var createNamespaceSample = new CreateNamespaceSample();
        var result = createNamespaceSample.CreateNamespace(_fixture.ProjectId, _fixture.LocationId, namespaceId);

        // Get the namespace.
        var namespaceName =
            NamespaceName.FromProjectLocationNamespace(_fixture.ProjectId, _fixture.LocationId, namespaceId);
        RegistrationServiceClient registrationServiceClient = RegistrationServiceClient.Create();
        var namespaceVal = registrationServiceClient.GetNamespace(namespaceName);

        Assert.Contains(namespaceVal.Name, result.Name);
    }
}