using System;
using System.Collections.Generic;
using Mustache;
using Xunit;

namespace Mustache.Test {
public class SectionsSpec {
/*
sections

Section tags and End Section tags are used in combination to wrap a section
of the template for iteration

These tags' content MUST be a non-whitespace character sequence NOT
containing the current closing delimiter; each Section tag MUST be followed
by an End Section tag with the same content within the same section.

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

For each element in the data list, the element MUST be pushed onto the
context stack, the section MUST be rendered, and the element MUST be popped
off the context stack.

Section and End Section tags SHOULD be treated as standalone when
appropriate.
*/

[Fact]
public void TestSectionsTruthy() { 
    object data = new {boolean = true, };
    Dictionary<string, string> partials = null;
    var template = @"""{{#boolean}}This should be rendered.{{/boolean}}""";
    var expected = @"""This should be rendered.""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsFalsey() { 
    object data = new {boolean = false, };
    Dictionary<string, string> partials = null;
    var template = @"""{{#boolean}}This should not be rendered.{{/boolean}}""";
    var expected = @"""""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsContext() { 
    object data = new {context = new {name = @"Joe", }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{#context}}Hi {{name}}.{{/context}}""";
    var expected = @"""Hi Joe.""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsDeeplyNestedContexts() { 
    object data = new {a = new {one = 1, }, b = new {two = 2, }, c = new {three = 3, }, d = new {four = 4, }, e = new {five = 5, }, };
    Dictionary<string, string> partials = null;
    var template = @"{{#a}}
{{one}}
{{#b}}
{{one}}{{two}}{{one}}
{{#c}}
{{one}}{{two}}{{three}}{{two}}{{one}}
{{#d}}
{{one}}{{two}}{{three}}{{four}}{{three}}{{two}}{{one}}
{{#e}}
{{one}}{{two}}{{three}}{{four}}{{five}}{{four}}{{three}}{{two}}{{one}}
{{/e}}
{{one}}{{two}}{{three}}{{four}}{{three}}{{two}}{{one}}
{{/d}}
{{one}}{{two}}{{three}}{{two}}{{one}}
{{/c}}
{{one}}{{two}}{{one}}
{{/b}}
{{one}}
{{/a}}
";
    var expected = @"1
121
12321
1234321
123454321
1234321
12321
121
1
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsList() { 
    object data = new {list = new object[] {new {item = 1, }, new {item = 2, }, new {item = 3, }, }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{#list}}{{item}}{{/list}}""";
    var expected = @"""123""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsEmptyList() { 
    object data = new {list = new object[] {}, };
    Dictionary<string, string> partials = null;
    var template = @"""{{#list}}Yay lists!{{/list}}""";
    var expected = @"""""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsDoubled() { 
    object data = new {two = @"second", bool_ = true, };
    Dictionary<string, string> partials = null;
    var template = @"{{#bool_}}
* first
{{/bool_}}
* {{two}}
{{#bool_}}
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
public void TestSectionsNestedTruthy() { 
    object data = new {bool_ = true, };
    Dictionary<string, string> partials = null;
    var template = @"| A {{#bool_}}B {{#bool_}}C{{/bool_}} D{{/bool_}} E |";
    var expected = @"| A B C D E |";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsNestedFalsey() { 
    object data = new {bool_ = false, };
    Dictionary<string, string> partials = null;
    var template = @"| A {{#bool_}}B {{#bool_}}C{{/bool_}} D{{/bool_}} E |";
    var expected = @"| A  E |";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsContextMisses() { 
    object data = null;
    Dictionary<string, string> partials = null;
    var template = @"[{{#missing}}Found key 'missing'!{{/missing}}]";
    var expected = @"[]";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsImplicitIteratorString() { 
    object data = new {list = new object[] {@"a", @"b", @"c", @"d", @"e", }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{#list}}({{.}}){{/list}}""";
    var expected = @"""(a)(b)(c)(d)(e)""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsImplicitIteratorInteger() { 
    object data = new {list = new object[] {1, 2, 3, 4, 5, }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{#list}}({{.}}){{/list}}""";
    var expected = @"""(1)(2)(3)(4)(5)""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsImplicitIteratorDecimal() { 
    object data = new {list = new object[] {1.1, 2.2, 3.3, 4.4, 5.5, }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{#list}}({{.}}){{/list}}""";
    var expected = @"""(1.1)(2.2)(3.3)(4.4)(5.5)""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsImplicitIteratorArray() { 
    object data = new {list = new object[] {new object[] {1, 2, 3, }, new object[] {@"a", @"b", @"c", }, }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{#list}}({{#.}}{{.}}{{/.}}){{/list}}""";
    var expected = @"""(123)(abc)""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsDottedNamesTruthy() { 
    object data = new {a = new {b = new {c = true, }, }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{#a.b.c}}Here{{/a.b.c}}"" == ""Here""";
    var expected = @"""Here"" == ""Here""";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsDottedNamesFalsey() { 
    object data = new {a = new {b = new {c = false, }, }, };
    Dictionary<string, string> partials = null;
    var template = @"""{{#a.b.c}}Here{{/a.b.c}}"" == """"";
    var expected = @""""" == """"";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsDottedNamesBrokenChains() { 
    object data = new {a = new {}, };
    Dictionary<string, string> partials = null;
    var template = @"""{{#a.b.c}}Here{{/a.b.c}}"" == """"";
    var expected = @""""" == """"";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsSurroundingWhitespace() { 
    object data = new {boolean = true, };
    Dictionary<string, string> partials = null;
    var template = @" | {{#boolean}}	|	{{/boolean}} | 
";
    var expected = @" | 	|	 | 
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsInternalWhitespace() { 
    object data = new {boolean = true, };
    Dictionary<string, string> partials = null;
    var template = @" | {{#boolean}} {{! Important Whitespace }}
 {{/boolean}} | 
";
    var expected = @" |  
  | 
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsIndentedInlineSections() { 
    object data = new {boolean = true, };
    Dictionary<string, string> partials = null;
    var template = @" {{#boolean}}YES{{/boolean}}
 {{#boolean}}GOOD{{/boolean}}
";
    var expected = @" YES
 GOOD
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsStandaloneLines() { 
    object data = new {boolean = true, };
    Dictionary<string, string> partials = null;
    var template = @"| This Is
{{#boolean}}
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
public void TestSectionsIndentedStandaloneLines() { 
    object data = new {boolean = true, };
    Dictionary<string, string> partials = null;
    var template = @"| This Is
  {{#boolean}}
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
public void TestSectionsStandaloneLineEndings() { 
    object data = new {boolean = true, };
    Dictionary<string, string> partials = null;
    var template = @"|
{{#boolean}}
{{/boolean}}
|";
    var expected = @"|
|";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsStandaloneWithoutPreviousLine() { 
    object data = new {boolean = true, };
    Dictionary<string, string> partials = null;
    var template = @"  {{#boolean}}
#{{/boolean}}
/";
    var expected = @"#
/";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsStandaloneWithoutNewline() { 
    object data = new {boolean = true, };
    Dictionary<string, string> partials = null;
    var template = @"#{{#boolean}}
/
  {{/boolean}}";
    var expected = @"#
/
";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestSectionsPadding() { 
    object data = new {boolean = true, };
    Dictionary<string, string> partials = null;
    var template = @"|{{# boolean }}={{/ boolean }}|";
    var expected = @"|=|";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

} // class
} // namespace
