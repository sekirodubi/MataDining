using System.Diagnostics;
using SoulsFormats;

List<string> files = new();

foreach (var path in args)
{
    FileAttributes attr = File.GetAttributes(path);

    if (attr.HasFlag(FileAttributes.Directory))
    {
        files.AddRange(Directory.GetFiles(path, "*.msb.dcx"));
    }
    else
    {
        if (File.Exists(path) && path.EndsWith("msb.dcx"))
        {
            files.Add(path);
        }
    }
}

try
{
    List<string> enemyData = new();
    List<string> assetData = new();
    foreach (var path in files)
    {
        MSBE msb = MSBE.Read(path);
        string mapName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(path));

        enemyData.AddRange(DumpEnemyData(msb, mapName));
        assetData.AddRange(DumpAssetData(msb, mapName));
    }

    if (enemyData.Count > 0)
    {
        DumpListToCSV(enemyData, "enemyData_", @"MapName,Type,EntityID,ModelName,Name,CharaInitID,TalkID,NPCParamID,ThinkParamID,Position");
    }

    if (assetData.Count > 0)
    {
        DumpListToCSV(assetData, "assetData_", @"MapName,Type,EntityID,ModelName,Name,Position");
    }

} catch (Exception e)
{
    Console.WriteLine(e.ToString());
    Console.ReadLine();
}

static void DumpListToCSV(List<string> data, string filePrefix, string header)
{
    string windowsTempPath = Path.GetTempPath();
    var filePath = Path.Combine(windowsTempPath, "MataDining_" + filePrefix + DateTime.Now.ToFileTimeUtc() + ".csv");
    var file = File.CreateText(filePath);
    file.WriteLine(header);
    foreach (var enemy in data)
    {
        file.WriteLine(enemy);
    }

    file.Close();

    Process.Start("notepad.exe", filePath);
}

static List<string> DumpEnemyData(MSBE msb, string mapName)
{
    List<string> res = new();
    foreach (var chr in msb.Parts.DummyEnemies)
    {
        res.Add($"{mapName},Dummy,{chr.EntityID},{chr.ModelName},{chr.Name},{chr.CharaInitID},{chr.TalkID},{chr.NPCParamID},{chr.ThinkParamID},\"{chr.Position}\"");
    }

    foreach (var chr in msb.Parts.Enemies)
    {
        res.Add($"{mapName},Enemy,{chr.EntityID},{chr.ModelName},{chr.Name},{chr.CharaInitID},{chr.TalkID},{chr.NPCParamID},{chr.ThinkParamID},\"{chr.Position}\"");
    }
    return res;
}

static List<string> DumpAssetData(MSBE msb, string mapName)
{
    List<string> res = new();
    foreach (var asset in msb.Parts.Assets)
    {
        res.Add($"{mapName},Asset,{asset.EntityID},{asset.ModelName},{asset.Name},\"{asset.Position}\"");
    }

    foreach (var asset in msb.Parts.DummyAssets)
    {
        res.Add($"{mapName},Dummy,{asset.EntityID},{asset.ModelName},{asset.Name},\"{asset.Position}\"");
    }
    return res;
}