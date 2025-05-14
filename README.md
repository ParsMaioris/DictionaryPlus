## Overview

A thread-safe .NET library for storing multiple values per key.

## Features

- Store multiple values per key
- Thread-safe operations
- Automatic resizing
- Safe iteration with concurrent modification detection

## Usage

```csharp
// Create a new MultiMap
var map = new MultiMap<string, int>();

// Add multiple values for the same key
map.Add("scores", 85);
map.Add("scores", 92);
map.Add("scores", 78);

// Get all values for a key
var scores = map.GetValues("scores"); // Returns [85, 92, 78]

// Remove a specific value
map.RemoveValue("scores", 78);

// Remove a key and all its values
map.RemoveKey("scores");
```

## Requirements

- .NET 9.0

## License

MIT