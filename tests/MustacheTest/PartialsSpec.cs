using System;
using System.Collections.Generic;
using KLab.Mustache;
using Xunit;

namespace MustacheTest {
public class PartialsSpec {
/*
partials

Partial tags are used to expand an external template into the current
template.

The tag's content MUST be a non-whitespace character sequence NOT containing
the current closing delimiter.

This tag's content names the partial to inject.  Set Delimiter tags MUST NOT
affect the parsing of a partial.  The partial MUST be rendered against the
context stack local to the tag.  If the named partial cannot be found, the
empty string SHOULD be used instead, as in interpolations.

Partial tags SHOULD be treated as standalone when appropriate.  If this tag
is used standalone, any whitespace preceding the tag should treated as
indentation, and prepended to each line of the partial before rendering.
*/

[Fact]
public void TestPartialsBasicBehavior() { 
    object data = null;
    var partials = new Dictionary<string, string>() {
        {@"text", @"from partial"},
    };
    var template = @"""{{>text}}""";
    var expected = @"""from partial""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestPartialsFailedLookup() { 
    object data = null;
    var partials = new Dictionary<string, string>() {
    };
    var template = @"""{{>text}}""";
    var expected = @"""""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestPartialsContext() { 
    object data = new {text = @"content", };
    var partials = new Dictionary<string, string>() {
        {@"partial", @"*{{text}}*"},
    };
    var template = @"""{{>partial}}""";
    var expected = @"""*content*""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestPartialsRecursion() { 
    object data = new {content = @"X", nodes = new object[] {new {content = @"Y", nodes = new object[] {}, }, }, };
    var partials = new Dictionary<string, string>() {
        {@"node", @"{{content}}<{{#nodes}}{{>node}}{{/nodes}}>"},
    };
    var template = @"{{>node}}";
    var expected = @"X<Y<>>";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestPartialsSurroundingWhitespace() { 
    object data = null;
    var partials = new Dictionary<string, string>() {
        {@"partial", @"	|	"},
    };
    var template = @"| {{>partial}} |";
    var expected = @"| 	|	 |";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestPartialsInlineIndentation() { 
    object data = new {data = @"|", };
    var partials = new Dictionary<string, string>() {
        {@"partial", @">
>"},
    };
    var template = @"  {{data}}  {{> partial}}
";
    var expected = @"  |  >
>
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestPartialsStandaloneLineEndings() { 
    object data = null;
    var partials = new Dictionary<string, string>() {
        {@"partial", @">"},
    };
    var template = @"|
{{>partial}}
|";
    var expected = @"|
>|";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestPartialsStandaloneWithoutPreviousLine() { 
    object data = null;
    var partials = new Dictionary<string, string>() {
        {@"partial", @">
>"},
    };
    var template = @"  {{>partial}}
>";
    var expected = @"  >
  >>";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestPartialsStandaloneWithoutNewline() { 
    object data = null;
    var partials = new Dictionary<string, string>() {
        {@"partial", @">
>"},
    };
    var template = @">
  {{>partial}}";
    var expected = @">
  >
  >";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestPartialsStandaloneIndentation() { 
    object data = new {content = @"<
->", };
    var partials = new Dictionary<string, string>() {
        {@"partial", @"|
{{{content}}}
|
"},
    };
    var template = @"\
 {{>partial}}
/
";
    var expected = @"\
 |
 <
->
 |
/
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestPartialsPaddingWhitespace() { 
    object data = new {boolean = true, };
    var partials = new Dictionary<string, string>() {
        {@"partial", @"[]"},
    };
    var template = @"|{{> partial }}|";
    var expected = @"|[]|";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

} // class
} // namespace
