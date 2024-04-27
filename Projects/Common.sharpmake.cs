using System;
using Sharpmake;

[Fragment, Flags]
public enum MorningCoffeeConfig
{
    Debug = 1,
    Development = 2,
    Profile = 4,
    Final = 8
}

public class MorningCoffeeTarget : ITarget
{
    public DevEnv DevEnv;
    public Platform Platform;
    public MorningCoffeeConfig Config;
    public OutputType OutputType;
    public Blob Blob;

    public override Sharpmake.Optimization GetOptimization()
    {
        switch (Config)
        {
            case MorningCoffeeConfig.Debug:
                return Sharpmake.Optimization.Debug;
            case MorningCoffeeConfig.Development:
                return Sharpmake.Optimization.Release;
            case MorningCoffeeConfig.Profile:
                return Sharpmake.Optimization.Retail;
            case MorningCoffeeConfig.Final:
                return Sharpmake.Optimization.Retail;
            default:
                throw new NotSupportedException("MorningCoffee config value " + Config.ToString());
        }
    }
}

public class CommonProject : Project
{
    public string EngineRootPath= @"[project.SharpmakeCsPath]\..";

    public CommonProject()
        : base(typeof(MorningCoffeeTarget))
    {
        AddTargets(new MorningCoffeeTarget
        {
            DevEnv = DevEnv.vs2022,
            Platform = Platform.win64,
            Config = MorningCoffeeConfig.Debug | MorningCoffeeConfig.Development | MorningCoffeeConfig.Profile | MorningCoffeeConfig.Final,
            OutputType = OutputType.Lib,
            Blob = Blob.NoBlob 
        });
    }

    [Configure]
    public virtual void ConfigureAll(Project.Configuration conf, MorningCoffeeTarget target)
    {
        conf.ProjectFileName = "[project.Name]_[target.DevEnv]_[target.Platform]";
        conf.ProjectPath = SourceRootPath;

        conf.IntermediatePath = @"[project.EngineRootPath]\Build\[project.Name]\[target.Platform]_[target.Config]_[target.DevEnv]";
        conf.TargetPath = @"[project.EngineRootPath]\Bin";

        // TODO: enable TreatWarningsAsErrors when possible
        conf.Options.Add(Options.Vc.Linker.SubSystem.Windows);
        conf.Options.Add(Options.Vc.General.TreatWarningsAsErrors.Disable);
        conf.Options.Add(Options.Vc.Compiler.CppLanguageStandard.CPP17);
        conf.Options.Add(Options.Vc.Compiler.RTTI.Enable);
        conf.Options.Add(Options.Vc.Linker.GenerateMapFile.Disable);

        conf.Options.Add(new Sharpmake.Options.Vc.Compiler.DisableSpecificWarnings(
            "4201", //nonstandard extension used: nameless struct/union
            "4324"  //structure was padded due to alignment specifier
        ));

        conf.Defines.Add("_HAS_EXCEPTIONS=0");
        conf.Defines.Add("UNICODE");
        conf.Defines.Add("_UNICODE");
        conf.Defines.Add("NOMINMAX");
        conf.Defines.Add("_ITERATOR_DEBUG_LEVEL=0");

        if (target.Config == MorningCoffeeConfig.Debug)
        {
            conf.TargetFileSuffix = "_dbg";

            conf.DefaultOption = Options.DefaultTarget.Debug;
        }
        else if (target.Config == MorningCoffeeConfig.Development)
        {
            conf.TargetFileSuffix = "_dev";

            conf.DefaultOption = Options.DefaultTarget.Release;
        }
        else if (target.Config == MorningCoffeeConfig.Profile)
        {
            conf.TargetFileSuffix = "_prf";

            conf.DefaultOption = Options.DefaultTarget.Release;

            conf.Options.Add(Sharpmake.Options.Vc.Compiler.Optimization.MaximizeSpeed);
            conf.Options.Add(Sharpmake.Options.Vc.General.WholeProgramOptimization.LinkTime);
            conf.Options.Add(Sharpmake.Options.Vc.Linker.LinkTimeCodeGeneration.UseLinkTimeCodeGeneration);
            conf.Options.Add(Sharpmake.Options.Vc.Linker.EnableCOMDATFolding.RemoveRedundantCOMDATs);
            conf.Options.Add(Sharpmake.Options.Vc.Linker.Reference.EliminateUnreferencedData);
        }
        else if (target.Config == MorningCoffeeConfig.Final)
        {
            conf.TargetFileSuffix = "_fnl";

            conf.DefaultOption = Options.DefaultTarget.Release;

            conf.Options.Add(Sharpmake.Options.Vc.Compiler.Optimization.MaximizeSpeed);
            conf.Options.Add(Sharpmake.Options.Vc.General.WholeProgramOptimization.LinkTime);
            conf.Options.Add(Sharpmake.Options.Vc.Linker.LinkTimeCodeGeneration.UseLinkTimeCodeGeneration);
            conf.Options.Add(Sharpmake.Options.Vc.Linker.EnableCOMDATFolding.RemoveRedundantCOMDATs);
            conf.Options.Add(Sharpmake.Options.Vc.Linker.Reference.EliminateUnreferencedData);
        }

		// TODO this should probably be referenced only from Engine lib
		conf.ReferencesByNuGetPackage.Add("WinPixEventRuntime", "1.0.240308001");
    }
}
