
using Slint;
using AppWindow;
using System.Device.Gpio;
using Iot.Device.RotaryEncoder;
using Iot.Device.Button;
using System.Device.Gpio.Drivers;
using SharpHook;
using SharpHook.Native;

#if IOT
var _tmpCounter = 1f;
// pin configuration
var gpioDriver = new LibGpiodDriver();
var gpioController = new GpioController(PinNumberingScheme.Logical, gpioDriver);

var _encoderClick = new GpioButton(
    buttonPin: 25,
    isPullUp: true,
    hasExternalResistor: false,
    gpioController
);

var encoder = new QuadratureRotaryEncoder(
    pinA: 7, // CLK
    pinB: 8, // DT
    edges: PinEventTypes.Rising,
    pulsesPerRotation: 20,
    gpioController
) {
    Debounce = TimeSpan.FromMilliseconds(2)
};
#endif

var keySimulator = new EventSimulator();
var win = new Window();

Console.WriteLine("Hello Torizon!");

// start the focus at the first button
Slint.Timer.Start(TimerMode.SingleShot, 2000, () => {
    keySimulator.SimulateKeyPress(KeyCode.VcTab);
    keySimulator.SimulateKeyRelease(KeyCode.VcTab);
});

#if IOT
// encoder events
_encoderClick.Press += (s, e) => {
    win.RunOnUiThread(() => {
        // press enter on the actual element focused
        keySimulator.SimulateKeyPress(KeyCode.VcEnter);
        keySimulator.SimulateKeyRelease(KeyCode.VcEnter);
    });
};

encoder.PulseCountChanged += (s, e) => {
    Console.WriteLine($"Value:: {e.Value}");

    win.RunOnUiThread(() => {
        // use the tmpCounter to check if we go up or down
        if (_tmpCounter > e.Value) {
            // next
            keySimulator.SimulateKeyPress(KeyCode.VcTab);
            keySimulator.SimulateKeyRelease(KeyCode.VcTab);
        } else {
            // previous
            keySimulator.SimulateKeyPress(KeyCode.VcLeftShift);
            keySimulator.SimulateKeyPress(KeyCode.VcTab);
            keySimulator.SimulateKeyRelease(KeyCode.VcTab);
            keySimulator.SimulateKeyRelease(KeyCode.VcLeftShift);
        }

        _tmpCounter = (float)e.Value;
    });
};
#endif

// Run Forrest, Run!
win.Run();
