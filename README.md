# Attempt to isolate an issue with dynamic components being rendered zillions of times a second

OK, so maybe that's a slight exaggeration, but it's a lot!

We are writing a Blazor application that needs to mimic a multi-window interface. We added a window manager and some base classes, and were able to create the individual windows, and have the window manager handle them correctly. Windows could be opened, moved and closed, and their positions restored next time the app was started.

Before anyone credits us with super-intelligence, we had a major head start from [a blog post about handling dynamic components in Blazor](https://mariomucalo.com/dynamic-components-in-blazor-with-parameters-and-event-handling/). We still had a major amount of work on top of that, but it was a huge help in getting going.

Our window manager maintained an `ObservableCollection` of a `WindowParameters` type, which is basically what he showed in the blog post (albeit with lots of extra code to handle the window events, etc), and rendered these as follows...

```c#
@foreach (WindowParameters w in Windows) {
  Console.WriteLine($"WindowManager loop - {DateTime.Now.ToLongTimeString()}");
  <DynamicComponent Type="@w.Type" Parameters="@w.GetDictionary()" />
}

@code {
  public ObservableCollection<WindowParameters> Windows { get; set; } = [];
}
```

The `Console.WriteLine` wasn't there originally, we added it when we realised that there was a problem.

According to our understanding, this code should only do anything when the collection was changed, ie a new window added or removed. Indeed, if we added the following code...

```c#
protected override void OnInitialized() =>
  Windows.CollectionChanged += Windows_CollectionChanged;

private void Windows_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
  Console.WriteLine($"WindowManager.Windows_CollectionChanged - {DateTime.Now.ToLongTimeString()}");
```

...we could see that the collection was indeed only changed at those times.

However, once we had added the `Console.WriteLine` in the `foreach` loop shown above, we saw output in the develoer console whizzing by as fast as the poor thing could manage.

Apart from any performance issues that this may incur, our main concern was that one of the windows contained child components (to keep each part of the window code small and manageable). These child components used `EventCallback` parameters to pass data back to the parent window, which updated local variables that were used as parameters to other child components. The problem was that when such an `EventCallback` was raised, the local variable was updated, the `SetParametersAsync` event in the child components was being called repeatedly. However, the parameter values were all `null` after the very first call, breaking the component behaviour. We traced the issue to the loop shown above.

So, here we are trying to isolate the issue, in order to work out what's going on.
