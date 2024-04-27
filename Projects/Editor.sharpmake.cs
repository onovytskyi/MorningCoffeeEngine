using Sharpmake;

[module: Sharpmake.Include("Common.sharpmake.cs")]
[module: Sharpmake.Include("Engine.sharpmake.cs")]

[Sharpmake.Generate]
public class MorningCoffeeEditor : CommonProject
{
    public MorningCoffeeEditor()
    {
        Name = "MorningCoffeeEditor";

        AddTargets(new MorningCoffeeTarget
        {
            DevEnv = DevEnv.vs2022,
            Platform = Platform.win64,
            Config = MorningCoffeeConfig.Debug | MorningCoffeeConfig.Development | MorningCoffeeConfig.Profile | MorningCoffeeConfig.Final,
            OutputType = OutputType.Lib,
            Blob = Blob.NoBlob
        });

        SourceRootPath = @"[project.SharpmakeCsPath]\..\Editor";

		SourceFilesExtensions.Add(".rc");
        SourceFilesExtensions.Add(".ico");
    }

    public override void ConfigureAll(Configuration conf, MorningCoffeeTarget target)
    {
        base.ConfigureAll(conf, target);

        // TODO rearrange source files on disk and remove all this bullshit
        //conf.SourceFilesBuildExcludeRegex.Add(Util.RegexPathCombine(@"Externals", @".*$"));
        //conf.SourceFilesBuildExcludeRegex.Add(Util.RegexPathCombine(@"FAudio", @".*$"));
        //conf.SourceFilesBuildExcludeRegex.Add(Util.RegexPathCombine(@"basis_universal", @".*$"));

        conf.SourceFilesBuildExclude.Add(@"main_SDL2.cpp");
        conf.SourceFilesBuildExclude.Add(@"App_Windows.cpp");
        //conf.SourceFilesBuildExclude.Add(@"Utility\volk.c");
        //conf.SourceFilesBuildExclude.Add(@"Utility\mikktspace.c");

        //conf.IncludePaths.Add(@"[project.SourceRootPath]\..\WickedEngine");
        //conf.IncludePaths.Add(@"[project.SourceRootPath]\Utility");

        //conf.Defines.Add("FAUDIO_WIN32_PLATFORM");

        // TODO this should be probably moved elsewhere
        conf.Output = Configuration.OutputType.Exe;
        //conf.TargetLibraryPath = @"[project.EngineRootPath]\L

		conf.AddPublicDependency<MorningCoffeeEngine>(target);
    }
}
