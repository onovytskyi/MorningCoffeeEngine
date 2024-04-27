using Sharpmake;

[module: Sharpmake.Include("Editor.sharpmake.cs")]

[Sharpmake.Generate]
public class MorningCoffeeSolution : Sharpmake.Solution
{
    public MorningCoffeeSolution()
        : base(typeof(MorningCoffeeTarget))
    {
        Name = "MorningCoffee";

        AddTargets(new MorningCoffeeTarget
        {
            DevEnv = DevEnv.vs2022,
            Platform = Platform.win64,
            Config = MorningCoffeeConfig.Debug | MorningCoffeeConfig.Development | MorningCoffeeConfig.Profile | MorningCoffeeConfig.Final,
            OutputType = OutputType.Lib,
            Blob = Blob.NoBlob 
        });
    }

    [Configure()]
    public void ConfigureAll(Configuration conf, MorningCoffeeTarget target)
    {
        conf.Name = "[target.Config]";
        conf.SolutionFileName = "[solution.Name]_[target.DevEnv]_[target.Platform]";
        conf.SolutionPath = @"[solution.SharpmakeCsPath]\..";

        conf.AddProject<MorningCoffeeEditor>(target);
    }
}

public static class Main
{
    [Sharpmake.Main]
    public static void SharpmakeMain(Sharpmake.Arguments arguments)
    {
        KitsRootPaths.SetUseKitsRootForDevEnv(DevEnv.vs2022, KitsRootEnum.KitsRoot10, Options.Vc.General.WindowsTargetPlatformVersion.Latest);

        arguments.Generate<MorningCoffeeSolution>();
    }
}
