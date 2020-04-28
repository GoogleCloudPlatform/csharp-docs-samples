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

using Xunit;
using Google.Cloud.Iam.V1;
using Google.Cloud.SecretManager.V1;

[Collection(nameof(SecretManagerFixture))]
public class IamGrantAccessTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly IamGrantAccessSample _sample;

    public IamGrantAccessTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new IamGrantAccessSample();
    }

    [Fact]
    public void GrantsAccess()
    {
        SecretName secretName = _fixture.SecretToCreateName;
        Policy result = _sample.IamGrantAccess(
          projectId: secretName.ProjectId, secretId: secretName.SecretId,
          member: "group:test@google.com");
        Assert.Contains(result.Bindings,
            binding => binding.Role == "roles/secretmanager.secretAccessor" && binding.Members.Contains("group:test@google.com"));
    }
}
