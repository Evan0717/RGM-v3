using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Exiled.API.Features;
using RGM.Variables;

namespace RGM.API.Features;

// Classes
public class DevManager
{
    /// <summary>
    /// <c>DevClass</c> Attribute를 사용하는 모든 Services/Features를 로드합니다.
    /// <br />
    /// <b>특정 모드와 연계됬을 경우 오류가 발생할 수 있습니다.</b>
    /// </summary>
    public static void Initialize()
    {
        return;
        
        // TODO: 미리보기로 공개, 로직 작동 불가.

        var assembly = Assembly.GetExecutingAssembly();

        var scannedTypes = assembly.GetTypes()
            .Select(type => new
            {
                Type = type,
                Attributes = type.GetCustomAttributes<DevClassAttribute>()
            })
            .Where(x =>
                x.Attributes != null &&
                !Variable.DevBlockedAttributes.Contains(x.Type.CustomAttributes.GetType())).ToList();

        if (!scannedTypes.Any()) return;

        foreach (var items in scannedTypes)
        {
            try
            {
                if (items?.Type == null ||
                    items.Attributes == null ||
                    Variable.DevScannedTypes.Contains((items.Type, items.Attributes))) continue;

                items.Attributes.FirstOrDefault(x => string.IsNullOrEmpty(x.Info.Name))!.Info.Name = items.Type.Name;
                items.Attributes.FirstOrDefault(x => x.Info.Version == null)!.Info.Version = new Version(0, 0, 0, 0);

                if (items.Attributes.Any(x => x.Info.ActiveNow))
                {
                    if (items.Attributes.All(x => x.Type == DevType.DevModeOnly) &&
                        !Main.Instance.Config.IsDevMode) continue;
                    var instance = (IDevClass)Activator.CreateInstance(items.Type);
                    Run(instance);
                    Variable.DevInstances.Add(items.Attributes.First(_ => true).Info.Name, instance);
                }

                Variable.DevScannedTypes.Add((items.Type, items.Attributes));
            }
            catch (Exception e)
            {
                Log.Error($"개발 클래스 {nameof(items)}를 로드하는 도중 오류가 발생하였습니다. \n {e}");
            }
        }

        Log.Info($"총 {scannedTypes.Count}개의 개발 클래스를 스캔하였습니다.");
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
            _ => ConsoleColor.White
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
        return;
        // TODO: 미리보기로 공개, 아직 작동하지 않음.

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
                    .Where(x => x.IsDefined(typeof(EnabledMethodAttribute)))
                    .ToList()
                    .ForEach(x => x.Invoke(instance, null));
            }
            else
            {
                Log.Info($"개발 클래스 {nameof(instance)}의 비활성화를 시도중");
                instance
                    .GetType()
                    .GetMethods()
                    .Where(x => x.IsDefined(typeof(DisabledMethodAttribute)))
                    .ToList()
                    .ForEach(x => x.Invoke(instance, null));
            }
        }
        catch (Exception e)
        {
            Log.Warn($"개발 클래스 {nameof(instance)}의 활성화를 실패하였습니다.\n{e}");
            throw e!.InnerException!;
        }
    }

    internal static void Remove(string name)
    {
        // TODO: 미리보기로 공개, 아직 작동하지 않음

        if (!Variable.DevInstances.TryGetValue(name, out var instance)) return;
        Run(instance, true);
    }
}

public class DevAspectManager<T> : DispatchProxy where T : IDevClass
{
    private T _obj;

    public DevAspectManager(T target, out T proxy)
    {
        throw new NotImplementedException("완성안됬슈");
        if (target.GetType().CustomAttributes.All(x => x.GetType() != typeof(DevDebugAttribute)))
            throw new ArgumentException(
                $"Class {nameof(target)} is cannot be found {nameof(DevDebugAttribute)} Attributes.");

        var proxia = Create<T, DevAspectManager<T>>();
        ((DevAspectManager<T>)(object)proxia)._obj = target;
        proxy = proxia;
    }

    protected override object Invoke(MethodInfo targetMethod, object[] args)
    {
        StringBuilder builder = new();

        builder.Append($"""
                        Method Name: {targetMethod.Name}
                        Method Position: {targetMethod.DeclaringType?.FullName}
                        Arguments: ({string.Join(", ", args.Select(x => x?.ToString() ?? "null"))}

                        """);
        try
        {
            var result = targetMethod.Invoke(_obj, args);
            builder.Append($"RESULT: {ConsoleColor.Green}SUCCESS{ConsoleColor.White}");
            Log.Info(builder.ToString());
            return result;
        }
        catch (Exception e)
        {
            builder.Append($"""
                               RESULT: {ConsoleColor.Red}FAILED{ConsoleColor.White}
                               -----------------------------------[Exception]-----------------------------------
                            """);
            Log.Info(builder.ToString());
            Log.Error(e);
            Log.Info("--------------------------------------[END]--------------------------------------");
            throw e.InnerException!;
        }
    }
}

public interface IDevClass
{
    void Active();

    void Disable();
}

// Attributes
[AttributeUsage(AttributeTargets.Class)]
public class DevClassAttribute : Attribute
{
    public DevType Type { get; }

    public DevInfo Info { get; set; }

    public DevClassAttribute(string name = null,
        DevType type = DevType.DevModeOnly,
        string version = null,
        string description = "",
        bool activeNow = false)
    {
        Type = type;

        if (!Version.TryParse(string.IsNullOrEmpty(version) ? "0.0.0.0" : version, out var result))
            Info = new DevInfo
            {
                Name = name,
                Description = description,
                ActiveNow = activeNow,
                Version = new Version(0, 0, 0, 0)
            };
        else
            Info = new DevInfo
            {
                Name = name,
                Description = description,
                ActiveNow = activeNow,
                Version = result
            };
    }
}

public class DevInfo
{
    public string Name { get; set; } = "";

    public Version Version { get; set; } = new(0, 0, 0);

    public string Description { get; set; } = "";

    public bool ActiveNow { get; init; }
}

[AttributeUsage(AttributeTargets.Method)]
public class EnabledMethodAttribute : Attribute;

[AttributeUsage(AttributeTargets.Method)]
public class DisabledMethodAttribute : Attribute;

[AttributeUsage(AttributeTargets.Method)]
public class DevDebugAttribute : Attribute;

// Enums
public enum DevType : byte
{
    ReleaseCandidate = 0,
    Beta = 1,
    Alpha = 2,
    Dev = 3,
    Canary = 4,
    DevModeOnly = 5
}

/*[DevClass("테스트", DevType.Alpha, "9.3.9", description: "유?출", true)]
public class TestDevClass : IDevClass
{
    [EnabledMethod]
    public void Active()
    {
        Log.Info("응애 :)");
    }

    [DisabledMethod]
    public void Disable()
    {
        Log.Info("힝... :(");
    }
}*/