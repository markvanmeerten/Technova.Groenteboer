# Groenteboer.Technova

Groenteboer.Technova is een kleine .NET class library voor Technova-studenten.

De library maakt het eenvoudiger om hardware te gebruiken zonder zelf met USB, HID of andere technische details bezig te zijn. De eerste ondersteunde hardware is de **DYMO M10 USB-weegschaal**.

De API is bewust simpel:

- je maakt een weegschaal aan;
- je koppelt events aan;
- je start de weegschaal;
- je reageert op nieuwe gewichten of statuswijzigingen.

## Namespace

Gebruik deze namespace:

```csharp
using Groenteboer.Technova.Devices.Scales;
```

## Weegschaal aanmaken

De makkelijkste manier is via de `ScaleFactory`:

```csharp
IScale scale = ScaleFactory.CreateDefault();
```

`CreateDefault()` probeert eerst een DYMO M10 te vinden. Als die niet aangesloten is, gebruikt de testapp een testweegschaal.

## Console voorbeeld

```csharp
using Groenteboer.Technova.Devices.Scales;

using IScale scale = ScaleFactory.CreateDefault();

scale.WeightChanged += (sender, e) =>
{
    Console.WriteLine($"Gewicht: {e.Weight} {e.Unit}");
};

scale.StatusChanged += (sender, e) =>
{
    Console.WriteLine($"Status: {e.Status}");
};

scale.ErrorOccurred += (sender, e) =>
{
    Console.WriteLine($"Fout: {e.Message}");
};

scale.Start();

Console.WriteLine("Druk op Enter om te stoppen...");
Console.ReadLine();
```

## WinForms voorbeeld

In WinForms mag je labels en andere controls alleen aanpassen vanaf de UI-thread.

De events van de weegschaal komen vanuit een achtergrondtaak. Daarom gebruik je `Invoke()` voordat je een label wijzigt.

```csharp
using Groenteboer.Technova.Devices.Scales;

public partial class Form1 : Form
{
    private readonly IScale _scale = ScaleFactory.CreateDefault();

    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        _scale.WeightChanged += Scale_WeightChanged;
        _scale.StatusChanged += Scale_StatusChanged;
        _scale.ErrorOccurred += Scale_ErrorOccurred;

        _scale.Start();
    }

    private void Scale_WeightChanged(object? sender, ScaleWeightChangedEventArgs e)
    {
        Invoke(() =>
        {
            lblWeight.Text = e.ToString();
        });
    }

    private void Scale_StatusChanged(object? sender, ScaleStatusChangedEventArgs e)
    {
        Invoke(() =>
        {
            lblStatus.Text = e.Status switch
            {
                ScaleStatus.Disconnected => "Niet verbonden",
                ScaleStatus.Standby => "Stand-by",
                ScaleStatus.Ready => "Klaar voor gebruik",
                _ => e.Status.ToString()
            };
        });
    }

    private void Scale_ErrorOccurred(object? sender, ScaleErrorEventArgs e)
    {
        Invoke(() =>
        {
            lblStatus.Text = e.Message;
        });
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        _scale.Dispose();
    }
}
```

## Statussen

Een weegschaal heeft drie mogelijke statussen:

```csharp
public enum ScaleStatus
{
    Disconnected,
    Standby,
    Ready
}
```

Betekenis:

- `Disconnected`: er is geen USB-verbinding of de weegschaal is gestopt.
- `Standby`: de weegschaal is wel aangesloten, maar staat uit of slaapt.
- `Ready`: de weegschaal is aangesloten en kan wegen.

## Events

### WeightChanged

Wordt aangeroepen als het gewicht verandert.

```csharp
scale.WeightChanged += (sender, e) =>
{
    Console.WriteLine(e.Weight);
    Console.WriteLine(e.Unit);
};
```

### StatusChanged

Wordt aangeroepen als de status verandert.

```csharp
scale.StatusChanged += (sender, e) =>
{
    Console.WriteLine(e.Status);
};
```

### ErrorOccurred

Wordt aangeroepen als er een fout optreedt.

```csharp
scale.ErrorOccurred += (sender, e) =>
{
    Console.WriteLine(e.Message);
};
```

## Starten en stoppen

Start de weegschaal:

```csharp
scale.Start();
```

Stop de weegschaal:

```csharp
scale.Stop();
```

Als je klaar bent, gebruik dan `Dispose()` of een `using` statement:

```csharp
scale.Dispose();
```

of:

```csharp
using IScale scale = ScaleFactory.CreateDefault();
```

## Belangrijk bij WinForms

Gebruik altijd `Invoke()` als je vanuit een scale-event een control aanpast:

```csharp
Invoke(() =>
{
    lblWeight.Text = e.ToString();
});
```

Zonder `Invoke()` kan WinForms een fout geven, omdat de weegschaal op een achtergrondthread wordt uitgelezen.
