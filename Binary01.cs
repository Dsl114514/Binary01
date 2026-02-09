using BepInEx;
using BepInEx.Hacknet;
using ZeroCracker;
using NPS;
using SKB;
namespace Binary01;

[BepInPlugin(ModGUID, ModName, ModVer)]
public class HacknetPluginTemplate : HacknetPlugin
{
    public const string ModGUID = "com.Binary01.Dsl";
    public const string ModName = "Binary01";
    public const string ModVer = "0.0.1";

    public override bool Load()
    {
        string asciiArt = @"
+=======================================================+
|     ____   _                              ___   _     |
|    | __ ) (_) _ __    __ _  _ __  _   _  / _ \ / |    | 
|    |  _ \ | || '_ \  / _` || '__|| | | || | | || |    | 
|    | |_) || || | | || (_| || |   | |_| || |_| || |    |
|    |____/ |_||_| |_| \__,_||_|    \__, | \___/ |_|    |
|                                   |___/               |
|                    #Version0.0.1#                     |
|                    #Loading.....#                     |                    
+=======================================================+
                               ";
        Console.WriteLine(asciiArt);
        Pathfinder.Daemon.DaemonManager.RegisterDaemon<NuclearPowerStation>();
        Pathfinder.Executable.ExecutableManager.RegisterExecutable<ZeroCrackers>("#PF_ZERO_EXE#");
        Pathfinder.Executable.ExecutableManager.RegisterExecutable<SMBcracker>("#PF_SMB_EXE#");
        Pathfinder.Port.PortManager.RegisterPort("smb", "Server Message Block", 445);
        return true;


    }
}
