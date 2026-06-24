# Groenteboer.Technova

Groenteboer.Technova is a small .NET library with simple hardware abstractions for Technova student projects.

The first supported device is the DYMO M10 USB scale. The API is intentionally small and event-driven, so students can focus on application logic instead of USB/HID details.

## Basic usage

```csharp
using Groenteboer.Technova.Devices.Scales;

using IScale scale = ScaleFactory.CreateDefault();

scale.WeightChanged += (sender, e) =>
{
    Console.WriteLine($"{e.Weight} {e.Unit}");
};

scale.StatusChanged += (sender, e) =>
{
    Console.WriteLine(e.Message);
};

scale.ErrorOccurred += (sender, e) =>
{
    Console.WriteLine(e.Message);
};

scale.Start();
```

For UI applications, remember that events are raised from a background thread. Use the UI framework's invoke/dispatcher method before changing controls.
