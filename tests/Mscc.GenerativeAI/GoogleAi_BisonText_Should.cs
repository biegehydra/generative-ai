#if NET472_OR_GREATER || NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endif
using FluentAssertions;
using Mscc.GenerativeAI;
using Xunit;
using Xunit.Abstractions;

namespace Test.Mscc.GenerativeAI
{
    [Collection(nameof(ConfigurationFixture))]
    public class GoogleAiBisonTextShould
    {
        private readonly ITestOutputHelper _output;
        private readonly ConfigurationFixture _fixture;
        private readonly string _model = Model.BisonText;

        public GoogleAiBisonTextShould(ITestOutputHelper output, ConfigurationFixture fixture)
        {
            _output = output;
            _fixture = fixture;
        }

        [Fact]
        public void Initialize_GoogleAI()
        {
            // Arrange
            
            // Act
            var googleAi = new GoogleAI(apiKey: _fixture.ApiKey);
            
            // Assert
            googleAi.Should().NotBeNull();
        }

        [Fact]
        public void Initialize_Using_GoogleAI()
        {
            // Arrange
            var expected = Environment.GetEnvironmentVariable("GOOGLE_AI_MODEL") ?? _model;
            var googleAi = new GoogleAI(apiKey: _fixture.ApiKey);
            
            // Act
            var model = googleAi.GenerativeModel();

            // Assert
            model.Should().NotBeNull();
            model.Name.Should().Be($"models/{expected}");
        }

        [Fact]
        public void Initialize_EnvVars()
        {
            // Arrange
            Environment.SetEnvironmentVariable("GOOGLE_API_KEY", _fixture.ApiKey);
            var expected = Environment.GetEnvironmentVariable("GOOGLE_AI_MODEL") ?? _model;

            // Act
            var model = new GenerativeModel();

            // Assert
            model.Should().NotBeNull();
            model.Name.Should().Be($"models/{expected}");
        }

        [Fact]
        public void Initialize_Default_Model()
        {
            // Arrange
            var expected = Environment.GetEnvironmentVariable("GOOGLE_AI_MODEL") ?? _model;

            // Act
            var model = new GenerativeModel(apiKey: _fixture.ApiKey);

            // Assert
            model.Should().NotBeNull();
            model.Name.Should().Be($"models/{expected}");
        }

        [Fact]
        public void Initialize_Model()
        {
            // Arrange
            var expected = _model;

            // Act
            var model = new GenerativeModel(apiKey: _fixture.ApiKey, model: _model);

            // Assert
            model.Should().NotBeNull();
            model.Name.Should().Be($"models/{expected}");
        }

        [Fact]
        public async Task List_Models()
        {
            // Arrange
            var model = new GenerativeModel(apiKey: _fixture.ApiKey);

            // Act
            var sut = await model.ListModels();

            // Assert
            sut.Should().NotBeNull();
            sut.Should().NotBeNull().And.HaveCountGreaterThanOrEqualTo(1);
            sut.ForEach(x =>
            {
                _output.WriteLine($"Model: {x.DisplayName} ({x.Name})");
                x.SupportedGenerationMethods.ForEach(m => _output.WriteLine($"  Method: {m}"));
            });
        }

        [Fact]
        public async Task List_Models_Using_OAuth()
        {
            // Arrange
            var model = new GenerativeModel { AccessToken = _fixture.AccessToken };

            // Act
            var sut = await model.ListModels();

            // Assert
            sut.Should().NotBeNull();
            sut.Should().NotBeNull().And.HaveCountGreaterThanOrEqualTo(1);
            sut.ForEach(x =>
            {
                _output.WriteLine($"Model: {x.DisplayName} ({x.Name})");
                x.SupportedGenerationMethods.ForEach(m => _output.WriteLine($"  Method: {m}"));
            });
        }

        [Fact]
        public async Task List_Tuned_Models()
        {
            // Arrange
            var model = new GenerativeModel { AccessToken = _fixture.AccessToken };

            // Act
            var sut = await model.ListModels(true);
            // var sut = await model.ListTunedModels();

            // Assert
            sut.Should().NotBeNull();
            sut.Should().NotBeNull().And.HaveCountGreaterThanOrEqualTo(1);
            sut.ForEach(x =>
            {
                _output.WriteLine($"Model: {x.DisplayName} ({x.Name})");
                x.TuningTask.Snapshots.ForEach(m => _output.WriteLine($"  Snapshot: {m}"));
            });
        }

        [Theory]
        [InlineData(Model.Gemini10Pro001)]
        [InlineData(Model.GeminiProVision)]
        [InlineData(Model.BisonText)]
        [InlineData(Model.BisonChat)]
        [InlineData("tunedModels/number-generator-model-psx3d3gljyko")]
        public async Task Get_Model_Information(string modelName)
        {
            // Arrange
            var model = new GenerativeModel(apiKey: _fixture.ApiKey);

            // Act
            var sut = await model.GetModel(model: modelName);

            // Assert
            sut.Should().NotBeNull();
            // sut.Name.Should().Be($"models/{modelName}");
            _output.WriteLine($"Model: {sut.DisplayName} ({sut.Name})");
            sut.SupportedGenerationMethods.ForEach(m => _output.WriteLine($"  Method: {m}"));
        }

        [Theory]
        [InlineData("tunedModels/number-generator-model-psx3d3gljyko")]
        public async Task Get_TunedModel_Information_Using_ApiKey(string modelName)
        {
            // Arrange
            var model = new GenerativeModel(apiKey: _fixture.ApiKey);

            
            // Act & Assert
            await Assert.ThrowsAsync<NotSupportedException>(() => model.GetModel(model: modelName));
        }

        [Theory]
        [InlineData(Model.Gemini10Pro001)]
        [InlineData(Model.GeminiProVision)]
        [InlineData(Model.BisonText)]
        [InlineData(Model.BisonChat)]
        [InlineData("tunedModels/number-generator-model-psx3d3gljyko")]
        public async Task Get_Model_Information_Using_OAuth(string modelName)
        {
            // Arrange
            var model = new GenerativeModel { AccessToken = _fixture.AccessToken };
            var expected = modelName;
            if (!expected.Contains("/"))
                expected = $"models/{expected}";

            // Act
            var sut = await model.GetModel(model: modelName);

            // Assert
            sut.Should().NotBeNull();
            sut.Name.Should().Be(expected);
            _output.WriteLine($"Model: {sut.DisplayName} ({sut.Name})");
            if (sut.State is null)
            {
                sut?.SupportedGenerationMethods?.ForEach(m => _output.WriteLine($"  Method: {m}"));
            }
            else
            {
                _output.WriteLine($"State: {sut.State}");
            }
        }

        [Fact]
        public async Task Generate_Content()
        {
            // Arrange
            var prompt = "Write a story about a magic backpack.";
            var model = new GenerativeModel(apiKey: _fixture.ApiKey, model: _model);

            // Act
            var response = await model.GenerateText(prompt);

            // Assert
            response.Should().NotBeNull();
            response.Candidates.Should().NotBeNull().And.HaveCount(1);
            response.Text.Should().NotBeEmpty();
            _output.WriteLine(response?.Text);
        }

        [Fact]
        public async Task Generate_Content_Request()
        {
            // Arrange
            var prompt = "Write a story about a magic backpack.";
            var model = new GenerativeModel(apiKey: _fixture.ApiKey, model: _model);
            var request = new GenerateTextRequest
            {
                Prompt = new TextPrompt()
                {
                    Text = prompt
                }
            };

            // Act
            var response = await model.GenerateText(request);

            // Assert
            response.Should().NotBeNull();
            response.Candidates.Should().NotBeNull().And.HaveCount(1);
            response.Text.Should().NotBeEmpty();
            _output.WriteLine(response?.Text);
        }

        [Fact]
        public async Task Generate_Content_RequestConstructor()
        {
            // Arrange
            var prompt = "Write a story about a magic backpack.";
            var model = new GenerativeModel(apiKey: _fixture.ApiKey, model: _model);
            var request = new GenerateTextRequest(prompt);

            // Act
            var response = await model.GenerateText(request);

            // Assert
            response.Should().NotBeNull();
            response.Candidates.Should().NotBeNull().And.HaveCount(1);
            response.Text.Should().NotBeEmpty();
            _output.WriteLine(response?.Text);
        }

        [Theory]
        [InlineData("How are you doing today?", 6)]
        [InlineData("What kind of fish is this?", 7)]
        [InlineData("Write a story about a magic backpack.", 8)]
        [InlineData("Write an extended story about a magic backpack.", 9)]
        public async Task Count_Tokens(string prompt, int expected)
        {
            // Arrange
            var model = new GenerativeModel(apiKey: _fixture.ApiKey, model: _model);

            // Act
            var response = await model.CountTokens(prompt);

            // Assert
            response.Should().NotBeNull();
            response.TotalTokens.Should().Be(expected);
            _output.WriteLine($"Tokens: {response?.TotalTokens}");
        }

        [Theory]
        [InlineData("How are you doing today?", 6)]
        [InlineData("What kind of fish is this?", 7)]
        [InlineData("Write a story about a magic backpack.", 8)]
        [InlineData("Write an extended story about a magic backpack.", 9)]
        public async Task Count_Tokens_Request(string prompt, int expected)
        {
            // Arrange
            var model = new GenerativeModel(apiKey: _fixture.ApiKey, model: _model);
            var request = new GenerateTextRequest(prompt);

            // Act
            var response = await model.CountTokens(request);

            // Assert
            response.Should().NotBeNull();
            response.TotalTokens.Should().Be(expected);
            _output.WriteLine($"Tokens: {response?.TotalTokens}");
        }

        [Fact]
        public async Task Create_Tuned_Model()
        {
            // Arrange
            var model = new GenerativeModel(apiKey: null, model: _model)
            {
                AccessToken = _fixture.AccessToken, ProjectId = _fixture.ProjectId
            };
            var request = new CreateTunedModelRequest()
            {
                BaseModel = _model.SanitizeModelName(),
                DisplayName = "Autogenerated Test model",
                TuningTask = new()
                {
                    Hyperparameters = new() { BatchSize = 2, LearningRate = 0.001f, EpochCount = 3 },
                    TrainingData = new()
                    {
                        Examples = new()
                        {
                            Examples = new()
                            {
                                new TuningExample() { TextInput = "1", Output = "2" },
                                new TuningExample() { TextInput = "3", Output = "4" },
                                new TuningExample() { TextInput = "-3", Output = "-2" },
                                new TuningExample() { TextInput = "twenty two", Output = "twenty three" },
                                new TuningExample() { TextInput = "two hundred", Output = "two hundred one" },
                                new TuningExample() { TextInput = "ninety nine", Output = "one hundred" },
                                new TuningExample() { TextInput = "8", Output = "9" },
                                new TuningExample() { TextInput = "-98", Output = "-97" },
                                new TuningExample() { TextInput = "1,000", Output = "1,001" },
                                new TuningExample() { TextInput = "thirteen", Output = "fourteen" },
                                new TuningExample() { TextInput = "seven", Output = "eight" },
                            }
                        }
                    }
                }
            };
  
            // Act
            var response = await model.CreateTunedModel(request);
            
            // Assert
            response.Should().NotBeNull();
            response.Name.Should().NotBeNull();
            response.Metadata.Should().NotBeNull();
            _output.WriteLine($"Name: {response.Name}");
            _output.WriteLine($"Model: {response.Metadata.TunedModel} (Steps: {response.Metadata.TotalSteps})");
        }

        [Fact]
        public async Task Create_Tuned_Model_Simply()
        {
            // Arrange
            var model = new GenerativeModel(apiKey: null, model: _model)
            {
                AccessToken = _fixture.AccessToken, ProjectId = _fixture.ProjectId
            };
            var parameters = new HyperParameters() { BatchSize = 2, LearningRate = 0.001f, EpochCount = 3 };
            var dataset = new List<TuningExample>
            {    
                new() { TextInput = "1", Output = "2" },
                new() { TextInput = "3", Output = "4" },
                new() { TextInput = "-3", Output = "-2" },
                new() { TextInput = "twenty two", Output = "twenty three" },
                new() { TextInput = "two hundred", Output = "two hundred one" },
                new() { TextInput = "ninety nine", Output = "one hundred" },
                new() { TextInput = "8", Output = "9" },
                new() { TextInput = "-98", Output = "-97" },
                new() { TextInput = "1,000", Output = "1,001" },
                new() { TextInput = "thirteen", Output = "fourteen" },
                new() { TextInput = "seven", Output = "eight" },
            };
            var request = new CreateTunedModelRequest(_model, 
                "Simply autogenerated Test model",
                dataset,
                parameters);
            
            // Act
            var response = await model.CreateTunedModel(request);
            
            // Assert
            response.Should().NotBeNull();
            response.Name.Should().NotBeNull();
            response.Metadata.Should().NotBeNull();
            _output.WriteLine($"Name: {response.Name}");
            _output.WriteLine($"Model: {response.Metadata.TunedModel} (Steps: {response.Metadata.TotalSteps})");
        }
        
        [Fact]
        public async Task Delete_Tuned_Model()
        {
            // Arrange
            var modelName = "tunedModels/number-generator-model-psx3d3gljyko";     // see List_Tuned_Models for available options.
            var model = new GenerativeModel()
            {
                AccessToken = _fixture.AccessToken,
                ProjectId = _fixture.ProjectId
            };
            
            // Act
            var response = await model.DeleteTunedModel(modelName);
            
            // Assert
            response.Should().NotBeNull();
            _output.WriteLine(response);
        }

        [Theory]
        [InlineData("255", "256")]
        [InlineData("41", "42")]
        // [InlineData("five", "six")]
        // [InlineData("Six hundred thirty nine", "Six hundred forty")]
        public async Task Generate_Content_TunedModel(string prompt, string expected)
        {
            // Arrange
            var model = new GenerativeModel(apiKey: null, model: "tunedModels/autogenerated-test-model-48gob9c9v54p")
            {
                AccessToken = _fixture.AccessToken,
                ProjectId = _fixture.ProjectId
            };

            // Act
            var response = await model.GenerateContent(prompt);

            // Assert
            response.Should().NotBeNull();
            response.Candidates.Should().NotBeNull().And.HaveCount(1);
            response.Text.Should().NotBeEmpty();
            _output.WriteLine(response?.Text);
            response?.Text.Should().Be(expected);
        }
    }
}