class SideEffectsInFP
{
    public static void TestFuncs()
    {
        var funcs = new List<Func<int>>();
        var s = "";
        for (var i = 4; i < 7; i++)
        {
            funcs.Add(() => i);
        }

        foreach (var f in funcs)
        {
            s += f();
        }
        Console.WriteLine(s); //456 //777
    }

    public static void TestFuncs2()
    {
        var funcs = new List<Func<int>>();
        var s = "";
        for (var i = 4; i < 7; i++)
        {
            var j = i;
            funcs.Add(() => j);
        }
        foreach (var f in funcs)
        {
            s += f();
        }
        Console.WriteLine(s);
    }

    public static void TestFuncs3()
    {
        var funcs = new List<Func<int>>();
        var s = "";
        var j = 0;
        for (var i = 4; i < 7; i++)
        {
            j = i;
            funcs.Add(() => j);
        }
        foreach (var f in funcs)
        {
            s += f();
        }
        Console.WriteLine(s); /// 666
    }
}