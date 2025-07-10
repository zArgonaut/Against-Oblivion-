using NUnit.Framework;
using UnityEngine.TestTools;
using System.Collections;

public class ExemploPlayModeTest
{
    [UnityTest]
    public IEnumerator Espera1Segundo()
    {
        yield return new WaitForSeconds(1);
        Assert.IsTrue(true);
    }
}
