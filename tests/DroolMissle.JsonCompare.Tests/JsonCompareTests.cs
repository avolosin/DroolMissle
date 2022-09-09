using DroolMissle.JsonCompare;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace DroolMissle.Tests
{
    public class JsonCompareTests
    {
        [Fact]
        public void GuyFawkes()
        {
            var expected = new
            {
                UUID = Guid.NewGuid(),
                FirstName = "Guy",
                LastName = "Fawkes",
                Skills = new List<string>() { "cool", "stuff" },
                BirthDate = DateTime.Parse("1570-04-13"),
                Convictions = 1,
                RelatedPersons = new[]{
                    new{
                        Name="Maria Pulleyn",
                        Relationship="Spouse"
                    },
                    new{
                        Name="Edward Fawkes",
                        Relationship="Father"
                    },
                    new{
                        Name="Edith Fawkes",
                        Relationship="Mother"
                    },
                }
            };

            var actual = new
            {
                UUID = Guid.NewGuid(),
                FirstName = "Guy",
                LastName = "Fawkes",
                Skills = new List<string>() { "cool", "stuff" },
                BirthDate = DateTime.Parse("1570-04-13"),
                Convictions = 1,
                RelatedPersons = new[]{
                    new{
                        Name="Maria Pulleyn",
                        Relationship="Spouse"
                    },
                    new{
                        Name="Edward Fawkes",
                        Relationship="Father"
                    },
                    new{
                        Name="Edith Fawkes",
                        Relationship="Mother"
                    },
                }
            };


            var comparisonResults = JsonComparer.Compare(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));

            comparisonResults.Count.ShouldBe(13);
            comparisonResults.Count(c => c.IsMatch == false).ShouldBe(1, "Expected 1 Json Property to not match");
            comparisonResults.First(c => c.IsMatch == false).Token.ShouldBe("UUID", "The UUID of expected and actual should not have been reported as a match");
        }

        [Fact]
        public void JsonCompare_Guid_AnyGuid()
        {
            var expected = new
            {
                UUID = Guid.NewGuid(),
                FirstName = "Guy",
                LastName = "Fawkes",
                Skills = new List<string>() { "cool", "stuff" },
                BirthDate = DateTime.Parse("1570-04-13"),
                Convictions = 1,
            };

            var actual = new
            {
                UUID = Guid.NewGuid(),
                FirstName = "Guy",
                LastName = "Fawkes",
                Skills = new List<string>() { "cool", "stuff" },
                BirthDate = DateTime.Parse("1570-04-13"),
                Convictions = 1,
            };


            var matchCriteria = new List<TokenMatchCriteria>()
            {
                TokenMatchCriteria.AnyGuid("UUID")
            };

            var comparisonResults = JsonComparer.Compare(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual), matchCriteria.ToArray());

            comparisonResults.Count(c => c.IsMatch == false).ShouldBe(0, "All values should match since the only change was Guid and we added a TokenMatchCriteria for UUID");
        }

        [Fact]
        public void JsonCompare_Guid_NullGuid()
        {
            var expected = new
            {
                UUID = Guid.NewGuid(),
                FirstName = "Guy",
                LastName = "Fawkes",
                Skills = new List<string>() { "cool", "stuff" },
                BirthDate = DateTime.Parse("1570-04-13"),
                Convictions = 1,
            };

            var actual = new
            {
                FirstName = "Guy",
                LastName = "Fawkes",
                Skills = new List<string>() { "cool", "stuff" },
                BirthDate = DateTime.Parse("1570-04-13"),
                Convictions = 1,
            };


            var matchCriteria = new List<TokenMatchCriteria>()
            {
                TokenMatchCriteria.AnyGuid("UUID",true)
            };

            var comparisonResults = JsonComparer.Compare(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual), matchCriteria.ToArray());

            comparisonResults.Count(c => c.IsMatch == false).ShouldBe(0, "All values should match since the UUID field is allowed to be null");
        }

        [Fact]
        public void JsonCompare_Integer_NullInt()
        {
            var expected = new
            {
                UUID = Guid.NewGuid(),
                FirstName = "Guy",
                LastName = "Fawkes",
                Skills = new List<string>() { "cool", "stuff" },
                BirthDate = DateTime.Parse("1570-04-13"),
                Convictions = 1,
            };

            var actual = new
            {
                UUID = Guid.NewGuid(),
                FirstName = "Guy",
                LastName = "Fawkes",
                Skills = new List<string>() { "cool", "stuff" },
                BirthDate = DateTime.Parse("1570-04-13"),
            };


            var matchCriteria = new List<TokenMatchCriteria>()
            {
                TokenMatchCriteria.AnyInteger("Convictions",true)
            };

            var comparisonResults = JsonComparer.Compare(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual), matchCriteria.ToArray());

            comparisonResults.Count(c => c.IsMatch == false).ShouldBe(0, "All values should match since the Convictions field is allowed to be null");
        }

        [Fact]
        public void JsonCompare_Bool_NullBool()
        {
            var expected = new
            {
                UUID = Guid.NewGuid(),
                FirstName = "Guy",
                LastName = "Fawkes",
                Skills = new List<string>() { "cool", "stuff" },
                BirthDate = DateTime.Parse("1570-04-13"),
                KnownId = false
            };

            var actual = new
            {
                UUID = Guid.NewGuid(),
                FirstName = "Guy",
                LastName = "Fawkes",
                Skills = new List<string>() { "cool", "stuff" },
                BirthDate = DateTime.Parse("1570-04-13"),
            };


            var matchCriteria = new List<TokenMatchCriteria>()
            {
                TokenMatchCriteria.AnyBoolean("KnownId",true)
            };

            var comparisonResults = JsonComparer.Compare(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual), matchCriteria.ToArray());

            comparisonResults.Count(c => c.IsMatch == false).ShouldBe(0, "All values should match since the KnownId field is allowed to be null");
        }

        [Fact]
        public void JsonCompare_Date_NullDate()
        {
            var expected = new
            {
                UUID = Guid.NewGuid(),
                FirstName = "Guy",
                LastName = "Fawkes",
                Skills = new List<string>() { "cool", "stuff" },
                BirthDate = DateTime.Parse("1570-04-13"),

            };

            var actual = new
            {
                UUID = Guid.NewGuid(),
                FirstName = "Guy",
                LastName = "Fawkes",
                Skills = new List<string>() { "cool", "stuff" },

            };


            var matchCriteria = new List<TokenMatchCriteria>()
            {
                TokenMatchCriteria.AnyDate("BirthDate",true)
            };

            var comparisonResults = JsonComparer.Compare(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual), matchCriteria.ToArray());

            comparisonResults.Count(c => c.IsMatch == false).ShouldBe(0, "All values should match since the BirthDate field is allowed to be null");
        }
    }
}
