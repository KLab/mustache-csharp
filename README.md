# MustacheC#

MustacheC# is an implementation of the mustache template engine in C#

## Installation

```
NuGet package is coming soon.
```

## Usage

Quick Example:
```csharp
object data = new { subject = "world" };
var template = "Hello, {{subject}}!";
var result = new MustacheRenderer().Render(template, data);
console.WriteLine(result); // => "Hello world!"
```

See the manual [mustache(5)](https://mustache.github.io/mustache.5.html) for further information.

## Running the tests

Run all tests:
```sh
cd tests/Mustache.Test
dotnet test
```

### spec
* `spec` directory is a copy of https://github.com/mustache/spec
* `spec/make-csharp.py` generates csharp test codes.

## Thanks

This project based on following projects.

* https://github.com/mustache/mustache
* https://github.com/mustache/spec
* https://github.com/Olivine-Labs/lustache

## License

This project is licensed under the terms of the MIT license.

