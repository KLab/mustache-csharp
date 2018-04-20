using System;
using System.Collections.Generic;
using KLab.Mustache;
using Xunit;

namespace MustacheTest {
public class LambdasSpec {
/*
~lambdas

Lambdas are a special-cased data type for use in interpolations and
sections.

When used as the data value for an Interpolation tag, the lambda MUST be
treatable as an arity 0 function, and invoked as such.  The returned value
MUST be rendered against the default delimiters, then interpolated in place
of the lambda.

When used as the data value for a Section tag, the lambda MUST be treatable
as an arity 1 function, and invoked as such (passing a String containing the
unprocessed section contents).  The returned value MUST be rendered against
the current delimiters, then interpolated in place of the section.
*/

[Fact]
public void TestLambdasInterpolation() { 
    object data = new {lambda = (Func<string>) (() => "world"), };
    Dictionary<string, string> partials = null;
    var template = @"Hello, {{lambda}}!";
    var expected = @"Hello, world!";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestLambdasInterpolationExpansion() { 
    object data = new {planet = @"world", lambda = (Func<string>) (() => "{{planet}}"), };
    Dictionary<string, string> partials = null;
    var template = @"Hello, {{lambda}}!";
    var expected = @"Hello, world!";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestLambdasInterpolationAlternateDelimiters() { 
    object data = new {planet = @"world", lambda = (Func<string>) (() => "|planet| => {{planet}}"), };
    Dictionary<string, string> partials = null;
    var template = @"{{= | | =}}
Hello, (|&lambda|)!";
    var expected = @"Hello, (|planet| => world)!";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestLambdasInterpolationMultipleCalls() { 
    int calls = 0;
    object data = new {lambda = (Func<string>) (() => { calls++; return calls.ToString(); }), };
    Dictionary<string, string> partials = null;
    var template = @"{{lambda}} == {{{lambda}}} == {{lambda}}";
    var expected = @"1 == 2 == 3";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestLambdasEscaping() { 
    object data = new {lambda = (Func<string>) (() => ">"), };
    Dictionary<string, string> partials = null;
    var template = @"<{{lambda}}{{{lambda}}}";
    var expected = @"<&gt;>";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestLambdasSection() { 
    object data = new {x = @"Error!", lambda = (Func<string, string>) (s => (s == "{{x}}") ? "yes" : "no"), };
    Dictionary<string, string> partials = null;
    var template = @"<{{#lambda}}{{x}}{{/lambda}}>";
    var expected = @"<yes>";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestLambdasSectionExpansion() { 
    object data = new {planet = @"Earth", lambda = (Func<string, string>) (txt => txt + "{{planet}}" + txt), };
    Dictionary<string, string> partials = null;
    var template = @"<{{#lambda}}-{{/lambda}}>";
    var expected = @"<-Earth->";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestLambdasSectionAlternateDelimiters() { 
    object data = new {planet = @"Earth", lambda = (Func<string, string>) (txt => txt + "{{planet}} => |planet|" + txt), };
    Dictionary<string, string> partials = null;
    var template = @"{{= | | =}}<|#lambda|-|/lambda|>";
    var expected = @"<-{{planet}} => Earth->";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestLambdasSectionMultipleCalls() { 
    object data = new {lambda = (Func<string, string>) (txt => "__" + txt + "__"), };
    Dictionary<string, string> partials = null;
    var template = @"{{#lambda}}FILE{{/lambda}} != {{#lambda}}LINE{{/lambda}}";
    var expected = @"__FILE__ != __LINE__";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

[Fact]
public void TestLambdasInvertedSection() { 
    object data = new {lambda = (Func<string, bool>) (txt => false), static_ = @"static", };
    Dictionary<string, string> partials = null;
    var template = @"<{{^lambda}}{{static_}}{{/lambda}}>";
    var expected = @"<>";
    var actual = new MustacheRenderer().Render(template, data, partials);
    Assert.Equal(expected, actual);
}

} // class
} // namespace
