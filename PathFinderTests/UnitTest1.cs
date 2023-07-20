using NUnit.Framework;
using System;
using System.Threading.Tasks;

[TestFixture]
public class PathFinderTest
{
    private string _ltPath = 
@"Klaipeda,Kaunas,2:00
Vilnius,Kaunas,1:00
Kaunas,Ukmerge,1:00
Vilnius,Ukmerge,1:00
Klaipeda,Panevezys,3:00
Panevezys,Vilnius,2:00
Vilnius,Utena,1:00
Utena,Klaipeda,5:00";
  
    [Test]
    public void FindFastestPath_LTPathArray_Success()
    {
        var finder = new PathFinder.PathFinder();
        var result = finder.FindFastestPath("Vilnius", "Klaipeda", _ltPath);
        Assert.That(result.Item1, Is.EqualTo(TimeSpan.FromMinutes(210)));
        Assert.That(result.Item2, Is.EqualTo("Vilnius,Kaunas,Klaipeda"));
    }
}