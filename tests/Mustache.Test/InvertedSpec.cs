using System;
using System.Collections.Generic;
using Mustache;
using Xunit;

namespace Mustache.Test {
public class InvertedSpec {
/*
inverted

Inverted Section tags and End Section tags are used in combination to wrap a
section of the template.

These tags' content MUST be a non-whitespace character sequence NOT
containing the current closing delimiter; each Inverted Section tag MUST be
followed by an End Section tag with the same content within the same
section.

This tag's content names the data to replace the tag.  Name resolution is as
follows:
  1) Split the name on periods; the first part is the name to resolve, any
  remaining parts should be retained.
  2) Walk the context stack from top to bottom, finding the first context
  that is a) a hash containing the name as a key OR b) an object responding
  to a method with the given name.
  3) If the context is a hash, the data is the value associated with the
  name.
  4) If the context is an object and the method with the given name has an
  arity of 1, the method SHOULD be called with a String containing the
  unprocessed contents of the sections; the data is the value returned.
  5) Otherwise, the data is the value returned by calling the method with
  the given name.
  6) If any name parts were retained in step 1, each should be resolved
  against a context stack containing only the result from the former
  resolution.  If any part fails resolution, the result should be considered
  falsey, and should interpolate as the empty string.
If the data is not of a list type, it is coerced into a list as follows: if
the data is truthy (e.g. `!!data == true`), use a single-element list
containing the data, otherwise use an empty list.

This section MUST NOT be rendered unless the data list is empty.

Inverted Section and End Section tags SHOULD be treated as standalone when
appropriate.
*/

[Fact]
public void TestInvertedFalsey() { 
    object data = new {boolean = false, };
    Dictionary<string, string> partials = null;
    var template = @"""{{^boolean}}This should be rendered.{{/boolean}}""";
    var expected = @"""This should be rendered.""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedTruthy() { 
    object data = new {boolean = true, };
    Dictionary<string, string> partials = null;
    var template = @"""{{^boolean}}This should not be rendered.{{/boolean}}""";
    var expected = @"""""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedContext() { 
    object data = new {context = new {name = @"Joe", }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{^context}}Hi {{name}}.{{/context}}""";
    var expected = @"""""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedList() { 
    object data = new {list = new object[] {new {n = 1, }, new {n = 2, }, new {n = 3, }, }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{^list}}{{n}}{{/list}}""";
    var expected = @"""""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedEmptyList() { 
    object data = new {list = new object[] {}, };
    Dictionary<string, string> partials = null;
    var template = @"""{{^list}}Yay lists!{{/list}}""";
    var expected = @"""Yay lists!""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedDoubled() { 
    object data = new {two = @"second", bool_ = false, };
    Dictionary<string, string> partials = null;
    var template = @"{{^bool_}}
* first
{{/bool_}}
* {{two}}
{{^bool_}}
* third
{{/bool_}}
";
    var expected = @"* first
* second
* third
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedNestedFalsey() { 
    object data = new {bool_ = false, };
    Dictionary<string, string> partials = null;
    var template = @"| A {{^bool_}}B {{^bool_}}C{{/bool_}} D{{/bool_}} E |";
    var expected = @"| A B C D E |";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedNestedTruthy() { 
    object data = new {bool_ = true, };
    Dictionary<string, string> partials = null;
    var template = @"| A {{^bool_}}B {{^bool_}}C{{/bool_}} D{{/bool_}} E |";
    var expected = @"| A  E |";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedContextMisses() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"[{{^missing}}Cannot find key 'missing'!{{/missing}}]";
    var expected = @"[Cannot find key 'missing'!]";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedDottedNamesTruthy() { 
    object data = new {a = new {b = new {c = true, }, }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{^a.b.c}}Not Here{{/a.b.c}}"" == """"";
    var expected = @""""" == """"";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedDottedNamesFalsey() { 
    object data = new {a = new {b = new {c = false, }, }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{^a.b.c}}Not Here{{/a.b.c}}"" == ""Not Here""";
    var expected = @"""Not Here"" == ""Not Here""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedDottedNamesBrokenChains() { 
    object data = new {a = new {}, };
    Dictionary<string, string> partials = null;
    var template = @"""{{^a.b.c}}Not Here{{/a.b.c}}"" == ""Not Here""";
    var expected = @"""Not Here"" == ""Not Here""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedSurroundingWhitespace() { 
    object data = new {boolean = false, };
    Dictionary<string, string> partials = null;
    var template = @" | {{^boolean}}	|	{{/boolean}} | 
";
    var expected = @" | 	|	 | 
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedInternalWhitespace() { 
    object data = new {boolean = false, };
    Dictionary<string, string> partials = null;
    var template = @" | {{^boolean}} {{! Important Whitespace }}
 {{/boolean}} | 
";
    var expected = @" |  
  | 
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedIndentedInlineSections() { 
    object data = new {boolean = false, };
    Dictionary<string, string> partials = null;
    var template = @" {{^boolean}}NO{{/boolean}}
 {{^boolean}}WAY{{/boolean}}
";
    var expected = @" NO
 WAY
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedStandaloneLines() { 
    object data = new {boolean = false, };
    Dictionary<string, string> partials = null;
    var template = @"| This Is
{{^boolean}}
|
{{/boolean}}
| A Line
";
    var expected = @"| This Is
|
| A Line
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedStandaloneIndentedLines() { 
    object data = new {boolean = false, };
    Dictionary<string, string> partials = null;
    var template = @"| This Is
  {{^boolean}}
|
  {{/boolean}}
| A Line
";
    var expected = @"| This Is
|
| A Line
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedStandaloneLineEndings() { 
    object data = new {boolean = false, };
    Dictionary<string, string> partials = null;
    var template = @"|
{{^boolean}}
{{/boolean}}
|";
    var expected = @"|
|";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedStandaloneWithoutPreviousLine() { 
    object data = new {boolean = false, };
    Dictionary<string, string> partials = null;
    var template = @"  {{^boolean}}
^{{/boolean}}
/";
    var expected = @"^
/";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedStandaloneWithoutNewline() { 
    object data = new {boolean = false, };
    Dictionary<string, string> partials = null;
    var template = @"^{{^boolean}}
/
  {{/boolean}}";
    var expected = @"^
/
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestInvertedPadding() { 
    object data = new {boolean = false, };
    Dictionary<string, string> partials = null;
    var template = @"|{{^ boolean }}={{/ boolean }}|";
    var expected = @"|=|";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

} // class
} // namespace
