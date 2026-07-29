using System;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using RGM.Variables;

namespace RGM.API.Features;

// Classes
[Obsolete("Under Construction")]
public class DevManager
{
    /// <summary>
    /// <c>DevClass</c> Attribute를 사용하는 모든 Services/Features를 로드합니다.
    /// <br />
    /// <b>특정 모드와 연계됬을 경우 오류가 발생할 수 있습니다.</b>
    /// </summary>
    public static void Initialize()
    {
        // TODO: 미리보기로 공개, 로직 작동 불가.
        return;
        
        var assembly = Assembly.GetExecutingAssembly();

        var scannedTypes = assembly.GetTypes()
            .Select(type => new
            {
                Type = type,
                Attributes = type.GetCustomAttributes<DevClassAttribute>()
            })
            .Where(x =>
                x.Attributes != null &&
                Variable.DevBlockedAttributes.Contains(x.Type.CustomAttributes.GetType())).ToList();
        
        if (!scannedTypes.Any()) return;
        
        Log.Info($"총 {scannedTypes.Count}개의 개발 클래스를 스캔함.");
        try
        {
            foreach (var items in scannedTypes)
            {
                if (items?.Type == null || items.Attributes == null) return;

                items.Attributes.First(x => string.IsNullOrEmpty(x.Name)).Name = items.Type.Name;
                items.Attributes.First(x => x.Version == null).Version = new Version(0, 0, 0, 0);

                if (items.Attributes.Any(x => x.ActiveNow))
                {
                    if (items.Attributes.All(x => x.Type == DevType.DevModeOnly) &&
                        !Main.Instance.Config.IsDevMode) continue;
                    var instance = Activator.CreateInstance(items.Type);
                    Run(instance);
                    Variable.DevInstances.Add(items.Attributes.First(_ => true).Name, instance);
                }

                Variable.DevScannedTypes.Add((items.Type, items.Attributes));
            }
        }
        catch (Exception e)
        {
            Log.Error($"스켄 도중 알 수 없는 오류 발생: {e.Message}");
            throw e.InnerException!;
        }
    }

    public static ConsoleColor GetDevColor(DevType type)
    {
        return type switch
        {
            DevType.DevModeOnly => ConsoleColor.Red,
            DevType.Canary => ConsoleColor.Yellow,
            DevType.Alpha => ConsoleColor.DarkYellow,
            DevType.Beta => ConsoleColor.Green,
            DevType.ReleaseCandidate => ConsoleColor.DarkGreen,
            DevType.Dev => ConsoleColor.Cyan,
            _ => ConsoleColor.DarkCyan
        };
    }

    /// <summary>
    /// 해당 기능을 실행합니다. <br />
    /// <c>DevOnEnabled</c> Attribute가 있을 경우 실행됩니다.
    /// </summary>
    /// <param name="instance">대상 인스턴스입니다.</param>
    /// <param name="isDisabled">비활성화 여부입니다. <c>DevOnDisabled</c> Attribute가 없을 경우 실행되지 않습니다.</param>
    public static void Run(object instance, bool isDisabled = false)
    {
        // TODO: 미리보기로 공개, 아직 작동하지 않음.
        return;
        
        if (!instance.GetType().IsClass ||
            instance.GetType().IsDefined(typeof(DevClassAttribute))) return;
        try
        {
            if (!isDisabled)
            {
                Log.Info($"개발 클래스 {nameof(instance)}의 활성화를 시도중");
                
                instance
                    .GetType()
                    .GetMethods()
                    .Where(x => x.IsDefined(typeof(DevOnEnabledAttribute)))
                    .ToList()
                    .ForEach(x => x.Invoke(instance, null));
            }
            else
            {
                Log.Info($"개발 클래스 {nameof(instance)}의 비활성화를 시도중");
                instance
                    .GetType()
                    .GetMethods()
                    .Where(x => x.IsDefined(typeof(DevOnDisabledAttribute)))
                    .ToList()
                    .ForEach(x => x.Invoke(instance, null));
            }
        }
        catch (Exception e)
        {
            Log.Warn($"개발 클래스 {nameof(instance)}의 활성화를 실패하였습니다.\n사유: {e.Message} \nStackTrace: {e.StackTrace}");
            throw e!.InnerException!;
        }
    }

    internal static void Remove(string name)
    {
        // TODO: 미리보기로 공개, 아직 작동하지 않음
        return;
        
        if (!Variable.DevInstances.TryGetValue(name, out var instance)) return;
        Run(instance, true);
    }
}

// Attributes
[Obsolete("Under Construction")]
[AttributeUsage(AttributeTargets.Class)]
public class DevClassAttribute(
    string name = null,
    DevType type = DevType.DevModeOnly,
    Version version = null,
    string description = "",
    bool activeNow = false) : Attribute
{
    public string Name { get; set; } = name;

    public DevType Type { get; set; } = type;

    public string Description { get; set; } = description;

    public Version Version { get; set; } = version;

    public bool ActiveNow { get; set; } = activeNow;
}

[Obsolete("Under Construction")]
[AttributeUsage(AttributeTargets.Method)]
public class DevOnEnabledAttribute : Attribute;

[Obsolete("Under Construction")]
[AttributeUsage(AttributeTargets.Method)]
public class DevOnDisabledAttribute : Attribute;

// Enums
[Obsolete("Under Construction")]
public enum DevType : byte
{
    ReleaseCandidate = 0,
    Beta = 1,
    Alpha = 2,
    Dev = 3,
    Canary = 4,
    DevModeOnly = 5
}