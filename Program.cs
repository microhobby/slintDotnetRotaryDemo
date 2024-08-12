using Slint;
using AppWindow;
using System.Device.Gpio;
using Iot.Device.RotaryEncoder;
using Iot.Device.Button;
using System.Device.Gpio.Drivers;

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

var win = new Window();

Console.WriteLine("Hello Torizon!");

// start the focus at the first button
Slint.Timer.Start(TimerMode.SingleShot, 1000, () => {
    win.counterFocus++;
    win.Change();
});

#if IOT
// encoder events
_encoderClick.Press += (s, e) => {
    win.RunOnUiThread(() => {
        win.Click();
    });
};

encoder.PulseCountChanged += (s, e) => {
    Console.WriteLine($"Value:: {e.Value}");

    win.RunOnUiThread(() => {
        // use the tmpCounter to check if we go up or down
        if (_tmpCounter > e.Value) {
            win.counterFocus++;
        } else {
            win.counterFocus--;
        }

        _tmpCounter = (float)e.Value;

        // normalize the values to be a rotation
        if (win.counterFocus < 1) {
            win.counterFocus = 4;
        } else if (win.counterFocus > 4) {
            win.counterFocus = 1;
        }

        win.Change();
    });
};
#endif

// Run Forrest, Run!
win.Run();
