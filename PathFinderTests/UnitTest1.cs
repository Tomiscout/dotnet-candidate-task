using NUnit.Framework;
using System;
using System.Threading.Tasks;

[TestFixture]
public class PathFinderTest
{
    private string _ltPath = @"
Klaipeda,Kaunas,2:00
Vilnius,Kaunas,1:00
Kaunas,Ukmerge,1:00
Vilnius,Ukmerge,1:00
Klaipeda,Panevezys,3:00
Panevezys,Vilnius,2:00";
  
    [Test]
    public async Task FindFastestPath_LTPathArray_Success()
    {
        var finder = new PathFinder.PathFinder();
        var result = await finder.FindFastestPath("Vilnius", "Klaipeda", _ltPath);
        Assert.AreEqual(TimeSpan.FromMinutes(210), result.Item1);
        Assert.AreEqual("Vilnius,Kaunas,Klaipeda", result.Item2);
    }
}