using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;


public struct ArcadeGamepadState : IInputStateTypeInfo
{
    // In the case of a HID (which we assume for the sake of this demonstration),
    // the format will be "HID". In practice, the format will depend on how your
    // particular device is connected and fed into the input system.
    // The format is a simple FourCC code that "tags" state memory blocks for the
    // device to give a base level of safety checks on memory operations.
    public FourCC format => new FourCC('H', 'I', 'D');

    [InputControl(name = "BlueBottom", layout = "Button", bit = 0, offset = 1)]
    [InputControl(name = "BlueTop", layout = "Button", bit = 1, offset = 1)]
    [InputControl(name = "GreenBottom", layout = "Button", bit = 2, offset = 1)]
    [InputControl(name = "GreenTop", layout = "Button", bit = 3, offset = 1)]
    [InputControl(name = "YellowBottom", layout = "Button", bit = 4, offset = 1)]
    [InputControl(name = "YellowTop", layout = "Button", bit = 5, offset = 1)]
    public int Buttons;


    const string processors = "CostumAxis";




    [InputControl(name ="stick",layout ="Vector2",format ="VC25", sizeInBits = 32,offset = 2, processors = "CostumVector,InvertVector2(invertX=true,invertY=false)")]
    [InputControl(name = "stick/x", format ="SHRT", processors = "CostumAxis,Invert")]
    public short StickX;
    [InputControl(name = "stick/y", format = "SHRT", offset = 2, processors = processors)]
    public short StickY;

}


#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
[InputControlLayout(displayName = "arcade Gamepad ", stateType = typeof(ArcadeGamepadState))]
public class ArcadeGamepad : InputDevice
{
    [InputControl]
    public ButtonControl GreenTop { get; private set; }
    [InputControl]
    public ButtonControl GreenBottom { get; private set; }

    [InputControl]
    public ButtonControl YellowTop { get; private set; }
    [InputControl]
    public ButtonControl YellowBottom { get; private set; }

    [InputControl]
    public ButtonControl BlueTop { get; private set; }
    [InputControl]
    public ButtonControl BlueBottom { get; private set; }

    [InputControl]
    public Vector2Control Stick { get; private set; }

    // Register the device.
    static ArcadeGamepad()
    {

        // In case you want instance of your device to automatically be created
        // when specific hardware is detected by the Unity runtime, you have to
        // add one or more "device matchers" (InputDeviceMatcher) for the layout.
        // These matchers are compared to an InputDeviceDescription received from
        // the Unity runtime when a device is connected. You can add them either
        // using InputSystem.RegisterLayoutMatcher() or by directly specifying a
        // matcher when registering the layout.

        InputSystem.RegisterProcessor<CostumAxisProcessor>("CostumAxis");
        InputSystem.RegisterProcessor<CostumVectorProcessor>("CostumVector");
        //return;
        InputSystem.RegisterLayout<ArcadeGamepad>(matches: new InputDeviceMatcher()
        .WithInterface("HID")
        .WithCapability("vendorId", 0x2341));
    }

    // This is only to trigger the static class constructor to automatically run
    // in the player.
    [RuntimeInitializeOnLoadMethod]
    private static void InitializeInPlayer() { }

    protected override void FinishSetup()
    {
        GreenTop = GetChildControl<ButtonControl>("GreenTop");
        GreenBottom = GetChildControl<ButtonControl>("GreenBottom");
        YellowTop = GetChildControl<ButtonControl>("YellowTop");
        YellowBottom = GetChildControl<ButtonControl>("YellowBottom");
        BlueTop = GetChildControl<ButtonControl>("BlueTop");
        BlueBottom = GetChildControl<ButtonControl>("BlueBottom");
        Stick = GetChildControl<Vector2Control>("stick");
        base.FinishSetup();
    }



}

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class CostumAxisProcessor : InputProcessor<float>
{
#if UNITY_EDITOR
    static CostumAxisProcessor()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {

    }

    public override float Process(float value, InputControl control)
        => Processor(value);

    public static float Processor(float value)
    {

        bool isZero = Mathf.RoundToInt(value) == -1;
        Debug.Log($"{Mathf.Round(value)}, {value > 0}");

        if (isZero)
            return 0;
        return value > 0 ? 1 : -1;
    }
}


#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class CostumVectorProcessor : InputProcessor<Vector2>
{
#if UNITY_EDITOR
    static CostumVectorProcessor()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {

    }

    public override Vector2 Process(Vector2 value, InputControl control)
    {

        return new Vector2(CostumAxisProcessor.Processor(value.x), CostumAxisProcessor.Processor(value.y));
    }
}

//...