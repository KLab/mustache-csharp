using System;
using System.Collections.Generic;
using KLab.Mustache;
using Xunit;

namespace MustacheTest {
public class InterpolationSpec {
/*
interpolation

Interpolation tags are used to integrate dynamic content into the template.

The tag's content MUST be a non-whitespace character sequence NOT containing
the current closing delimiter.

This tag's content names the data to replace the tag.  A single period (`.`)
indicates that the item currently sitting atop the context stack should be
used; otherwise, name resolution is as follows:
  1) Split the name on periods; the first part is the name to resolve, any
  remaining parts should be retained.
  2) Walk the context stack from top to bottom, finding the first context
  that is a) a hash containing the name as a key OR b) an object responding
  to a method with the given name.
  3) If the context is a hash, the data is the value associated with the
  name.
  4) If the context is an object, the data is the value returned by the
  method with the given name.
  5) If any name parts were retained in step 1, each should be resolved
  against a context stack containing only the result from the former
  resolution.  If any part fails resolution, the result should be considered
  falsey, and should interpolate as the empty string.
Data should be coerced into a string (and escaped, if appropriate) before
interpolation.

The Interpolation tags MUST NOT be treated as standalone.
*/

[Fact]
public void TestInterpolationNoInterpolation() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"Hello from {Mustache}!
";
    var expected = @"Hello from {Mustache}!
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationBasicInterpolation() { 
    object data = new {subject = @"world", };
    Dictionary<string, string> partials = null;
    var template = @"Hello, {{subject}}!
";
    var expected = @"Hello, world!
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationHtmlEscaping() { 
    object data = new {forbidden = @"& "" < >", };
    Dictionary<string, string> partials = null;
    var template = @"These characters should be HTML escaped: {{forbidden}}
";
    var expected = @"These characters should be HTML escaped: &amp; &quot; &lt; &gt;
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationTripleMustache() { 
    object data = new {forbidden = @"& "" < >", };
    Dictionary<string, string> partials = null;
    var template = @"These characters should not be HTML escaped: {{{forbidden}}}
";
    var expected = @"These characters should not be HTML escaped: & "" < >
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationAmpersand() { 
    object data = new {forbidden = @"& "" < >", };
    Dictionary<string, string> partials = null;
    var template = @"These characters should not be HTML escaped: {{&forbidden}}
";
    var expected = @"These characters should not be HTML escaped: & "" < >
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationBasicIntegerInterpolation() { 
    object data = new {mph = 85, };
    Dictionary<string, string> partials = null;
    var template = @"""{{mph}} miles an hour!""";
    var expected = @"""85 miles an hour!""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationTripleMustacheIntegerInterpolation() { 
    object data = new {mph = 85, };
    Dictionary<string, string> partials = null;
    var template = @"""{{{mph}}} miles an hour!""";
    var expected = @"""85 miles an hour!""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationAmpersandIntegerInterpolation() { 
    object data = new {mph = 85, };
    Dictionary<string, string> partials = null;
    var template = @"""{{&mph}} miles an hour!""";
    var expected = @"""85 miles an hour!""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationBasicDecimalInterpolation() { 
    object data = new {power = 1.21, };
    Dictionary<string, string> partials = null;
    var template = @"""{{power}} jiggawatts!""";
    var expected = @"""1.21 jiggawatts!""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationTripleMustacheDecimalInterpolation() { 
    object data = new {power = 1.21, };
    Dictionary<string, string> partials = null;
    var template = @"""{{{power}}} jiggawatts!""";
    var expected = @"""1.21 jiggawatts!""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationAmpersandDecimalInterpolation() { 
    object data = new {power = 1.21, };
    Dictionary<string, string> partials = null;
    var template = @"""{{&power}} jiggawatts!""";
    var expected = @"""1.21 jiggawatts!""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationBasicContextMissInterpolation() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"I ({{cannot}}) be seen!";
    var expected = @"I () be seen!";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationTripleMustacheContextMissInterpolation() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"I ({{{cannot}}}) be seen!";
    var expected = @"I () be seen!";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationAmpersandContextMissInterpolation() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"I ({{&cannot}}) be seen!";
    var expected = @"I () be seen!";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationDottedNamesBasicInterpolation() { 
    object data = new {person = new {name = @"Joe", }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{person.name}}"" == ""{{#person}}{{name}}{{/person}}""";
    var expected = @"""Joe"" == ""Joe""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationDottedNamesTripleMustacheInterpolation() { 
    object data = new {person = new {name = @"Joe", }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{{person.name}}}"" == ""{{#person}}{{{name}}}{{/person}}""";
    var expected = @"""Joe"" == ""Joe""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationDottedNamesAmpersandInterpolation() { 
    object data = new {person = new {name = @"Joe", }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{&person.name}}"" == ""{{#person}}{{&name}}{{/person}}""";
    var expected = @"""Joe"" == ""Joe""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationDottedNamesArbitraryDepth() { 
    object data = new {a = new {b = new {c = new {d = new {e = new {name = @"Phil", }, }, }, }, }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{a.b.c.d.e.name}}"" == ""Phil""";
    var expected = @"""Phil"" == ""Phil""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationDottedNamesBrokenChains() { 
    object data = new {a = new {}, };
    Dictionary<string, string> partials = null;
    var template = @"""{{a.b.c}}"" == """"";
    var expected = @""""" == """"";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationDottedNamesBrokenChainResolution() { 
    object data = new {a = new {b = new {}, }, c = new {name = @"Jim", }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{a.b.c.name}}"" == """"";
    var expected = @""""" == """"";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationDottedNamesInitialResolution() { 
    object data = new {a = new {b = new {c = new {d = new {e = new {name = @"Phil", }, }, }, }, }, b = new {c = new {d = new {e = new {name = @"Wrong", }, }, }, }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{#a}}{{b.c.d.e.name}}{{/a}}"" == ""Phil""";
    var expected = @"""Phil"" == ""Phil""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationInterpolationSurroundingWhitespace() { 
    object data = new {string_ = @"---", };
    Dictionary<string, string> partials = null;
    var template = @"| {{string_}} |";
    var expected = @"| --- |";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationTripleMustacheSurroundingWhitespace() { 
    object data = new {string_ = @"---", };
    Dictionary<string, string> partials = null;
    var template = @"| {{{string_}}} |";
    var expected = @"| --- |";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationAmpersandSurroundingWhitespace() { 
    object data = new {string_ = @"---", };
    Dictionary<string, string> partials = null;
    var template = @"| {{&string_}} |";
    var expected = @"| --- |";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationInterpolationStandalone() { 
    object data = new {string_ = @"---", };
    Dictionary<string, string> partials = null;
    var template = @"  {{string_}}
";
    var expected = @"  ---
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationTripleMustacheStandalone() { 
    object data = new {string_ = @"---", };
    Dictionary<string, string> partials = null;
    var template = @"  {{{string_}}}
";
    var expected = @"  ---
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationAmpersandStandalone() { 
    object data = new {string_ = @"---", };
    Dictionary<string, string> partials = null;
    var template = @"  {{&string_}}
";
    var expected = @"  ---
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationInterpolationWithPadding() { 
    object data = new {string_ = @"---", };
    Dictionary<string, string> partials = null;
    var template = @"|{{ string_ }}|";
    var expected = @"|---|";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationTripleMustacheWithPadding() { 
    object data = new {string_ = @"---", };
    Dictionary<string, string> partials = null;
    var template = @"|{{{ string_ }}}|";
    var expected = @"|---|";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInterpolationAmpersandWithPadding() { 
    object data = new {string_ = @"---", };
    Dictionary<string, string> partials = null;
    var template = @"|{{& string_ }}|";
    var expected = @"|---|";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

} // class
} // namespace
