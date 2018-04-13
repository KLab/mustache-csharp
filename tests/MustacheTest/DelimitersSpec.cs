using System;
using System.Collections.Generic;
using KLab.Mustache;
using Xunit;

namespace MustacheTest {
public class DelimitersSpec {
/*
delimiters

Set Delimiter tags are used to change the tag delimiters for all content
following the tag in the current compilation unit.

The tag's content MUST be any two non-whitespace sequences (separated by
whitespace) EXCEPT an equals sign ('=') followed by the current closing
delimiter.

Set Delimiter tags SHOULD be treated as standalone when appropriate.
*/

[Fact]
public void TestDelimitersPairBehavior() { 
    object data = new {text = @"Hey!", };
    Dictionary<string, string> partials = null;
    var template = @"{{=<% %>=}}(<%text%>)";
    var expected = @"(Hey!)";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestDelimitersSpecialCharacters() { 
    object data = new {text = @"It worked!", };
    Dictionary<string, string> partials = null;
    var template = @"({{=[ ]=}}[text])";
    var expected = @"(It worked!)";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestDelimitersSections() { 
    object data = new {section = true, data = @"I got interpolated.", };
    Dictionary<string, string> partials = null;
    var template = @"[
{{#section}}
  {{data}}
  |data|
{{/section}}

{{= | | =}}
|#section|
  {{data}}
  |data|
|/section|
]
";
    var expected = @"[
  I got interpolated.
  |data|

  {{data}}
  I got interpolated.
]
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestDelimitersInvertedSections() { 
    object data = new {section = false, data = @"I got interpolated.", };
    Dictionary<string, string> partials = null;
    var template = @"[
{{^section}}
  {{data}}
  |data|
{{/section}}

{{= | | =}}
|^section|
  {{data}}
  |data|
|/section|
]
";
    var expected = @"[
  I got interpolated.
  |data|

  {{data}}
  I got interpolated.
]
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestDelimitersPartialInheritence() { 
    object data = new {value = @"yes", };
    var partials = new Dictionary<string, string>() {
        {@"include", @".{{value}}."},
    };
    var template = @"[ {{>include}} ]
{{= | | =}}
[ |>include| ]
";
    var expected = @"[ .yes. ]
[ .yes. ]
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestDelimitersPostPartialBehavior() { 
    object data = new {value = @"yes", };
    var partials = new Dictionary<string, string>() {
        {@"include", @".{{value}}. {{= | | =}} .|value|."},
    };
    var template = @"[ {{>include}} ]
[ .{{value}}.  .|value|. ]
";
    var expected = @"[ .yes.  .yes. ]
[ .yes.  .|value|. ]
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestDelimitersSurroundingWhitespace() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"| {{=@ @=}} |";
    var expected = @"|  |";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestDelimitersOutlyingWhitespaceInline() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @" | {{=@ @=}}
";
    var expected = @" | 
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestDelimitersStandaloneTag() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"Begin.
{{=@ @=}}
End.
";
    var expected = @"Begin.
End.
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestDelimitersIndentedStandaloneTag() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"Begin.
  {{=@ @=}}
End.
";
    var expected = @"Begin.
End.
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestDelimitersStandaloneLineEndings() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"|
{{= @ @ =}}
|";
    var expected = @"|
|";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestDelimitersStandaloneWithoutPreviousLine() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"  {{=@ @=}}
=";
    var expected = @"=";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestDelimitersStandaloneWithoutNewline() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"=
  {{=@ @=}}";
    var expected = @"=
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestDelimitersPairWithPadding() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"|{{= @   @ =}}|";
    var expected = @"||";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

} // class
} // namespace
