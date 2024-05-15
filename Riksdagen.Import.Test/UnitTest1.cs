namespace Riksdagen.Import.Test
{
    public class UnitTest1
    {
        [Theory]
       // [InlineData("ha031.txt")]
        [InlineData("ha03100.txt")]
        [InlineData("ha031d3.txt")]
        [InlineData("ha031d5.txt")]
        [InlineData("ha0387.txt")]
        [InlineData("ha0352.txt")]
        [InlineData("ha0365.txt")]
        public void TestImport(string fileName)
        {
            var basePath = @"props\";
            var rows = File.ReadAllLines(basePath+fileName);
            Assert.True(rows.Any());
            var parser = new PropositionParserTxt(rows);
            var sections = parser.ParseSections();
            var summary = parser.GetSummary();
            
            foreach(var s in sections)
            {
                Assert.NotNull(s.Text);
            }
        }

        [Theory]
        [InlineData("3.12.13", 3.012013)]
        [InlineData("1.2.3", 1.002003)]
        [InlineData("3.10.1", 3.010001)]
        [InlineData("3.1", 3.001)]
        public void TestParsingSectionNumber(string sectionString, double val)
        {
            var result = PropositionParserTxt.ParseSectionNumber(sectionString);
            Assert.Equal(val, result.Value, 0.0000000001);
        }
    }
}