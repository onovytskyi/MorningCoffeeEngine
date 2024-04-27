using Sharpmake;

[module: Sharpmake.Include("Common.sharpmake.cs")]

[Sharpmake.Generate]
public class MorningCoffeeEngine : CommonProject
{
    public MorningCoffeeEngine()
    {
        Name = "MorningCoffeeEngine";

        AddTargets(new MorningCoffeeTarget
        {
            DevEnv = DevEnv.vs2022,
            Platform = Platform.win64,
            Config = MorningCoffeeConfig.Debug | MorningCoffeeConfig.Development | MorningCoffeeConfig.Profile | MorningCoffeeConfig.Final,
            OutputType = OutputType.Lib,
            Blob = Blob.NoBlob 
        });

        SourceRootPath = @"[project.SharpmakeCsPath]\..\WickedEngine";
    }

    public override void ConfigureAll(Configuration conf, MorningCoffeeTarget target)
    {
        base.ConfigureAll(conf, target);

        // TODO rearrange source files on disk and remove all this bullshit
        conf.SourceFilesBuildExcludeRegex.Add(Util.RegexPathCombine(@"Externals", @".*$"));
        conf.SourceFilesBuildExcludeRegex.Add(Util.RegexPathCombine(@"FAudio", @".*$"));
        conf.SourceFilesBuildExcludeRegex.Add(Util.RegexPathCombine(@"basis_universal", @".*$"));

        conf.SourceFilesBuildExclude.Add(@"offlineshadercompiler.cpp");
        conf.SourceFilesBuildExclude.Add(@"Utility\D3D12MemAlloc.cpp");
        conf.SourceFilesBuildExclude.Add(@"Utility\volk.c");
        conf.SourceFilesBuildExclude.Add(@"Utility\mikktspace.c");
        conf.SourceFilesBuildExclude.Add(@"LUA\lua.c");

        conf.IncludePaths.Add(SourceRootPath);
        conf.IncludePaths.Add(@"[project.SourceRootPath]\BULLET");
        conf.IncludePaths.Add(@"[project.SourceRootPath]\Utility");

        conf.Defines.Add("FAUDIO_WIN32_PLATFORM");

        // TODO this should be probably moved elsewhere
        conf.Output = Configuration.OutputType.Lib;
        conf.TargetLibraryPath = @"[project.EngineRootPath]\Lib";
    }
}
