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

    [InputControl(name = "BlueBottom", layout = "Button", bit = 0, offset = 3)]
    [InputControl(name = "BlueTop", layout = "Button", bit = 1, offset = 3)]
    [InputControl(name = "GreenBottom", layout = "Button", bit = 2, offset = 3)]
    [InputControl(name = "GreenTop", layout = "Button", bit = 3, offset = 3)]
    [InputControl(name = "YellowBottom", layout = "Button", bit = 4, offset = 3)]
    [InputControl(name = "YellowTop", layout = "Button", bit = 5, offset = 3)]
    public int Buttons;


    [InputControl(name = "Stick", layout = "Stick", offset = 1, sizeInBits = 16)]
    [InputControl(name = "Stick/x", layout = "Axis", offset = 0, sizeInBits = 8, format = "BIT",
          parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5",processors = "AxisDeadzone(min=0.2,max=0.8)")]
    [InputControl(name = "Stick/y", layout = "Axis", offset = 1, sizeInBits = 8, format = "BIT",
          parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5", processors = "AxisDeadzone(min=0.2,max=0.8),invert")]
    public short Stick;

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
        InputSystem.RegisterLayout<ArcadeGamepad>(matches: new InputDeviceMatcher()
        .WithInterface("HID")
        .WithCapability("vendorId", 0x16C0));
        //.WithCapability("productId", 0x5E1));
        return;

        InputSystem.RegisterLayout<ArcadeGamepad>(matches: new InputDeviceMatcher()
        .WithInterface("HID")
        .WithManufacturer("xin-mo.com"));
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
        Stick = GetChildControl<StickControl>("Stick");
        base.FinishSetup();
    }
}