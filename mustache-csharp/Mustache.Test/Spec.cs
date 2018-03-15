using System;
using Mustache;

namespace Mustache.Test {
public static class Specs {
/*
comments

Comment tags represent content that should never appear in the resulting
output.

The tag's content may contain any substring (including newlines) EXCEPT the
closing delimiter.

Comment tags SHOULD be treated as standalone when appropriate.
*/
public static void TestCommentsInline() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"12345{{! Comment Block! }}67890";
	var expected = @"1234567890";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Comment blocks should be removed from the template.");
	}
}
public static void TestCommentsMultiline() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"12345{{!
  This is a
  multi-line comment...
}}67890
";
	var expected = @"1234567890
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Multiline comments should be permitted.");
	}
}
public static void TestCommentsStandalone() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"Begin.
{{! Comment Block! }}
End.
";
	var expected = @"Begin.
End.
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"All standalone comment lines should be removed.");
	}
}
public static void TestCommentsIndentedStandalone() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"Begin.
  {{! Indented Comment Block! }}
End.
";
	var expected = @"Begin.
End.
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"All standalone comment lines should be removed.");
	}
}
public static void TestCommentsStandaloneLineEndings() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"|
{{! Standalone Comment }}
|";
	var expected = @"|
|";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"""\r\n"" should be considered a newline for standalone tags.");
	}
}
public static void TestCommentsStandaloneWithoutPreviousLine() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"  {{! I'm Still Standalone }}
!";
	var expected = @"!";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Standalone tags should not require a newline to precede them.");
	}
}
public static void TestCommentsStandaloneWithoutNewline() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"!
  {{! I'm Still Standalone }}";
	var expected = @"!
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Standalone tags should not require a newline to follow them.");
	}
}
public static void TestCommentsMultilineStandalone() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"Begin.
{{!
Something's going on here...
}}
End.
";
	var expected = @"Begin.
End.
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"All standalone comment lines should be removed.");
	}
}
public static void TestCommentsIndentedMultilineStandalone() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"Begin.
  {{!
    Something's going on here...
  }}
End.
";
	var expected = @"Begin.
End.
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"All standalone comment lines should be removed.");
	}
}
public static void TestCommentsIndentedInline() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"  12 {{! 34 }}
";
	var expected = @"  12 
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Inline comments should not strip whitespace");
	}
}
public static void TestCommentsSurroundingWhitespace() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"12345 {{! Comment Block! }} 67890";
	var expected = @"12345  67890";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Comment removal should preserve surrounding whitespace.");
	}
}

/*
delimiters

Set Delimiter tags are used to change the tag delimiters for all content
following the tag in the current compilation unit.

The tag's content MUST be any two non-whitespace sequences (separated by
whitespace) EXCEPT an equals sign ('=') followed by the current closing
delimiter.

Set Delimiter tags SHOULD be treated as standalone when appropriate.
*/
public static void TestDelimitersPairBehavior() { 
	object data = new {text = @"Hey!", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"{{=<% %>=}}(<%text%>)";
	var expected = @"(Hey!)";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"The equals sign (used on both sides) should permit delimiter changes.");
	}
}
public static void TestDelimitersSpecialCharacters() { 
	object data = new {text = @"It worked!", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"({{=[ ]=}}[text])";
	var expected = @"(It worked!)";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Characters with special meaning regexen should be valid delimiters.");
	}
}
public static void TestDelimitersSections() { 
	object data = new {section = true, data = @"I got interpolated.", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
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
	if (expected != actual) { 
		throw new Exception(@"Delimiters set outside sections should persist.");
	}
}
public static void TestDelimitersInvertedSections() { 
	object data = new {section = false, data = @"I got interpolated.", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
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
	if (expected != actual) { 
		throw new Exception(@"Delimiters set outside inverted sections should persist.");
	}
}
public static void TestDelimitersPartialInheritence() { 
	object data = new {value = @"yes", };
	var partials = new System.Collections.Generic.Dictionary<string, string>() {
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
	if (expected != actual) { 
		throw new Exception(@"Delimiters set in a parent template should not affect a partial.");
	}
}
public static void TestDelimitersPostPartialBehavior() { 
	object data = new {value = @"yes", };
	var partials = new System.Collections.Generic.Dictionary<string, string>() {
		{@"include", @".{{value}}. {{= | | =}} .|value|."},
	};
	var template = @"[ {{>include}} ]
[ .{{value}}.  .|value|. ]
";
	var expected = @"[ .yes.  .yes. ]
[ .yes.  .|value|. ]
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Delimiters set in a partial should not affect the parent template.");
	}
}
public static void TestDelimitersSurroundingWhitespace() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"| {{=@ @=}} |";
	var expected = @"|  |";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Surrounding whitespace should be left untouched.");
	}
}
public static void TestDelimitersOutlyingWhitespaceInline() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @" | {{=@ @=}}
";
	var expected = @" | 
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Whitespace should be left untouched.");
	}
}
public static void TestDelimitersStandaloneTag() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"Begin.
{{=@ @=}}
End.
";
	var expected = @"Begin.
End.
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Standalone lines should be removed from the template.");
	}
}
public static void TestDelimitersIndentedStandaloneTag() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"Begin.
  {{=@ @=}}
End.
";
	var expected = @"Begin.
End.
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Indented standalone lines should be removed from the template.");
	}
}
public static void TestDelimitersStandaloneLineEndings() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"|
{{= @ @ =}}
|";
	var expected = @"|
|";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"""\r\n"" should be considered a newline for standalone tags.");
	}
}
public static void TestDelimitersStandaloneWithoutPreviousLine() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"  {{=@ @=}}
=";
	var expected = @"=";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Standalone tags should not require a newline to precede them.");
	}
}
public static void TestDelimitersStandaloneWithoutNewline() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"=
  {{=@ @=}}";
	var expected = @"=
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Standalone tags should not require a newline to follow them.");
	}
}
public static void TestDelimitersPairWithPadding() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"|{{= @   @ =}}|";
	var expected = @"||";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Superfluous in-tag whitespace should be ignored.");
	}
}

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
public static void TestInterpolationNoInterpolation() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"Hello from {Mustache}!
";
	var expected = @"Hello from {Mustache}!
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Mustache-free templates should render as-is.");
	}
}
public static void TestInterpolationBasicInterpolation() { 
	object data = new {subject = @"world", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"Hello, {{subject}}!
";
	var expected = @"Hello, world!
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Unadorned tags should interpolate content into the template.");
	}
}
public static void TestInterpolationHtmlEscaping() { 
	object data = new {forbidden = @"& "" < >", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"These characters should be HTML escaped: {{forbidden}}
";
	var expected = @"These characters should be HTML escaped: &amp; &quot; &lt; &gt;
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Basic interpolation should be HTML escaped.");
	}
}
public static void TestInterpolationTripleMustache() { 
	object data = new {forbidden = @"& "" < >", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"These characters should not be HTML escaped: {{{forbidden}}}
";
	var expected = @"These characters should not be HTML escaped: & "" < >
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Triple mustaches should interpolate without HTML escaping.");
	}
}
public static void TestInterpolationAmpersand() { 
	object data = new {forbidden = @"& "" < >", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"These characters should not be HTML escaped: {{&forbidden}}
";
	var expected = @"These characters should not be HTML escaped: & "" < >
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Ampersand should interpolate without HTML escaping.");
	}
}
public static void TestInterpolationBasicIntegerInterpolation() { 
	object data = new {mph = 85, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{mph}} miles an hour!""";
	var expected = @"""85 miles an hour!""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Integers should interpolate seamlessly.");
	}
}
public static void TestInterpolationTripleMustacheIntegerInterpolation() { 
	object data = new {mph = 85, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{{mph}}} miles an hour!""";
	var expected = @"""85 miles an hour!""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Integers should interpolate seamlessly.");
	}
}
public static void TestInterpolationAmpersandIntegerInterpolation() { 
	object data = new {mph = 85, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{&mph}} miles an hour!""";
	var expected = @"""85 miles an hour!""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Integers should interpolate seamlessly.");
	}
}
public static void TestInterpolationBasicDecimalInterpolation() { 
	object data = new {power = 1.21, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{power}} jiggawatts!""";
	var expected = @"""1.21 jiggawatts!""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Decimals should interpolate seamlessly with proper significance.");
	}
}
public static void TestInterpolationTripleMustacheDecimalInterpolation() { 
	object data = new {power = 1.21, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{{power}}} jiggawatts!""";
	var expected = @"""1.21 jiggawatts!""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Decimals should interpolate seamlessly with proper significance.");
	}
}
public static void TestInterpolationAmpersandDecimalInterpolation() { 
	object data = new {power = 1.21, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{&power}} jiggawatts!""";
	var expected = @"""1.21 jiggawatts!""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Decimals should interpolate seamlessly with proper significance.");
	}
}
public static void TestInterpolationBasicContextMissInterpolation() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"I ({{cannot}}) be seen!";
	var expected = @"I () be seen!";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Failed context lookups should default to empty strings.");
	}
}
public static void TestInterpolationTripleMustacheContextMissInterpolation() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"I ({{{cannot}}}) be seen!";
	var expected = @"I () be seen!";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Failed context lookups should default to empty strings.");
	}
}
public static void TestInterpolationAmpersandContextMissInterpolation() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"I ({{&cannot}}) be seen!";
	var expected = @"I () be seen!";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Failed context lookups should default to empty strings.");
	}
}
public static void TestInterpolationDottedNamesBasicInterpolation() { 
	object data = new {person = new {name = @"Joe", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{person.name}}"" == ""{{#person}}{{name}}{{/person}}""";
	var expected = @"""Joe"" == ""Joe""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Dotted names should be considered a form of shorthand for sections.");
	}
}
public static void TestInterpolationDottedNamesTripleMustacheInterpolation() { 
	object data = new {person = new {name = @"Joe", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{{person.name}}}"" == ""{{#person}}{{{name}}}{{/person}}""";
	var expected = @"""Joe"" == ""Joe""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Dotted names should be considered a form of shorthand for sections.");
	}
}
public static void TestInterpolationDottedNamesAmpersandInterpolation() { 
	object data = new {person = new {name = @"Joe", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{&person.name}}"" == ""{{#person}}{{&name}}{{/person}}""";
	var expected = @"""Joe"" == ""Joe""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Dotted names should be considered a form of shorthand for sections.");
	}
}
public static void TestInterpolationDottedNamesArbitraryDepth() { 
	object data = new {a = new {b = new {c = new {d = new {e = new {name = @"Phil", }, }, }, }, }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{a.b.c.d.e.name}}"" == ""Phil""";
	var expected = @"""Phil"" == ""Phil""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Dotted names should be functional to any level of nesting.");
	}
}
public static void TestInterpolationDottedNamesBrokenChains() { 
	object data = new {a = new {}, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{a.b.c}}"" == """"";
	var expected = @""""" == """"";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Any falsey value prior to the last part of the name should yield ''.");
	}
}
public static void TestInterpolationDottedNamesBrokenChainResolution() { 
	object data = new {a = new {b = new {}, }, c = new {name = @"Jim", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{a.b.c.name}}"" == """"";
	var expected = @""""" == """"";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Each part of a dotted name should resolve only against its parent.");
	}
}
public static void TestInterpolationDottedNamesInitialResolution() { 
	object data = new {a = new {b = new {c = new {d = new {e = new {name = @"Phil", }, }, }, }, }, b = new {c = new {d = new {e = new {name = @"Wrong", }, }, }, }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{#a}}{{b.c.d.e.name}}{{/a}}"" == ""Phil""";
	var expected = @"""Phil"" == ""Phil""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"The first part of a dotted name should resolve as any other name.");
	}
}
public static void TestInterpolationInterpolationSurroundingWhitespace() { 
	object data = new {string_ = @"---", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"| {{string_}} |";
	var expected = @"| --- |";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Interpolation should not alter surrounding whitespace.");
	}
}
public static void TestInterpolationTripleMustacheSurroundingWhitespace() { 
	object data = new {string_ = @"---", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"| {{{string_}}} |";
	var expected = @"| --- |";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Interpolation should not alter surrounding whitespace.");
	}
}
public static void TestInterpolationAmpersandSurroundingWhitespace() { 
	object data = new {string_ = @"---", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"| {{&string_}} |";
	var expected = @"| --- |";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Interpolation should not alter surrounding whitespace.");
	}
}
public static void TestInterpolationInterpolationStandalone() { 
	object data = new {string_ = @"---", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"  {{string_}}
";
	var expected = @"  ---
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Standalone interpolation should not alter surrounding whitespace.");
	}
}
public static void TestInterpolationTripleMustacheStandalone() { 
	object data = new {string_ = @"---", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"  {{{string_}}}
";
	var expected = @"  ---
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Standalone interpolation should not alter surrounding whitespace.");
	}
}
public static void TestInterpolationAmpersandStandalone() { 
	object data = new {string_ = @"---", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"  {{&string_}}
";
	var expected = @"  ---
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Standalone interpolation should not alter surrounding whitespace.");
	}
}
public static void TestInterpolationInterpolationWithPadding() { 
	object data = new {string_ = @"---", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"|{{ string_ }}|";
	var expected = @"|---|";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Superfluous in-tag whitespace should be ignored.");
	}
}
public static void TestInterpolationTripleMustacheWithPadding() { 
	object data = new {string_ = @"---", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"|{{{ string_ }}}|";
	var expected = @"|---|";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Superfluous in-tag whitespace should be ignored.");
	}
}
public static void TestInterpolationAmpersandWithPadding() { 
	object data = new {string_ = @"---", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"|{{& string_ }}|";
	var expected = @"|---|";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Superfluous in-tag whitespace should be ignored.");
	}
}

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
public static void TestInvertedFalsey() { 
	object data = new {boolean = false, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{^boolean}}This should be rendered.{{/boolean}}""";
	var expected = @"""This should be rendered.""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Falsey sections should have their contents rendered.");
	}
}
public static void TestInvertedTruthy() { 
	object data = new {boolean = true, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{^boolean}}This should not be rendered.{{/boolean}}""";
	var expected = @"""""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Truthy sections should have their contents omitted.");
	}
}
public static void TestInvertedContext() { 
	object data = new {context = new {name = @"Joe", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{^context}}Hi {{name}}.{{/context}}""";
	var expected = @"""""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Objects and hashes should behave like truthy values.");
	}
}
public static void TestInvertedList() { 
	object data = new {list = new object[] {new {n = 1, }, new {n = 2, }, new {n = 3, }, }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{^list}}{{n}}{{/list}}""";
	var expected = @"""""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Lists should behave like truthy values.");
	}
}
public static void TestInvertedEmptyList() { 
	object data = new {list = new object[] {}, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{^list}}Yay lists!{{/list}}""";
	var expected = @"""Yay lists!""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Empty lists should behave like falsey values.");
	}
}
public static void TestInvertedDoubled() { 
	object data = new {two = @"second", bool_ = false, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
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
	if (expected != actual) { 
		throw new Exception(@"Multiple inverted sections per template should be permitted.");
	}
}
public static void TestInvertedNestedFalsey() { 
	object data = new {bool_ = false, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"| A {{^bool_}}B {{^bool_}}C{{/bool_}} D{{/bool_}} E |";
	var expected = @"| A B C D E |";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Nested falsey sections should have their contents rendered.");
	}
}
public static void TestInvertedNestedTruthy() { 
	object data = new {bool_ = true, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"| A {{^bool_}}B {{^bool_}}C{{/bool_}} D{{/bool_}} E |";
	var expected = @"| A  E |";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Nested truthy sections should be omitted.");
	}
}
public static void TestInvertedContextMisses() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"[{{^missing}}Cannot find key 'missing'!{{/missing}}]";
	var expected = @"[Cannot find key 'missing'!]";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Failed context lookups should be considered falsey.");
	}
}
public static void TestInvertedDottedNamesTruthy() { 
	object data = new {a = new {b = new {c = true, }, }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{^a.b.c}}Not Here{{/a.b.c}}"" == """"";
	var expected = @""""" == """"";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Dotted names should be valid for Inverted Section tags.");
	}
}
public static void TestInvertedDottedNamesFalsey() { 
	object data = new {a = new {b = new {c = false, }, }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{^a.b.c}}Not Here{{/a.b.c}}"" == ""Not Here""";
	var expected = @"""Not Here"" == ""Not Here""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Dotted names should be valid for Inverted Section tags.");
	}
}
public static void TestInvertedDottedNamesBrokenChains() { 
	object data = new {a = new {}, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{^a.b.c}}Not Here{{/a.b.c}}"" == ""Not Here""";
	var expected = @"""Not Here"" == ""Not Here""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Dotted names that cannot be resolved should be considered falsey.");
	}
}
public static void TestInvertedSurroundingWhitespace() { 
	object data = new {boolean = false, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @" | {{^boolean}}	|	{{/boolean}} | 
";
	var expected = @" | 	|	 | 
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Inverted sections should not alter surrounding whitespace.");
	}
}
public static void TestInvertedInternalWhitespace() { 
	object data = new {boolean = false, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @" | {{^boolean}} {{! Important Whitespace }}
 {{/boolean}} | 
";
	var expected = @" |  
  | 
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Inverted should not alter internal whitespace.");
	}
}
public static void TestInvertedIndentedInlineSections() { 
	object data = new {boolean = false, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @" {{^boolean}}NO{{/boolean}}
 {{^boolean}}WAY{{/boolean}}
";
	var expected = @" NO
 WAY
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Single-line sections should not alter surrounding whitespace.");
	}
}
public static void TestInvertedStandaloneLines() { 
	object data = new {boolean = false, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
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
	if (expected != actual) { 
		throw new Exception(@"Standalone lines should be removed from the template.");
	}
}
public static void TestInvertedStandaloneIndentedLines() { 
	object data = new {boolean = false, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
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
	if (expected != actual) { 
		throw new Exception(@"Standalone indented lines should be removed from the template.");
	}
}
public static void TestInvertedStandaloneLineEndings() { 
	object data = new {boolean = false, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"|
{{^boolean}}
{{/boolean}}
|";
	var expected = @"|
|";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"""\r\n"" should be considered a newline for standalone tags.");
	}
}
public static void TestInvertedStandaloneWithoutPreviousLine() { 
	object data = new {boolean = false, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"  {{^boolean}}
^{{/boolean}}
/";
	var expected = @"^
/";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Standalone tags should not require a newline to precede them.");
	}
}
public static void TestInvertedStandaloneWithoutNewline() { 
	object data = new {boolean = false, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"^{{^boolean}}
/
  {{/boolean}}";
	var expected = @"^
/
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Standalone tags should not require a newline to follow them.");
	}
}
public static void TestInvertedPadding() { 
	object data = new {boolean = false, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"|{{^ boolean }}={{/ boolean }}|";
	var expected = @"|=|";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Superfluous in-tag whitespace should be ignored.");
	}
}

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
public static void TestPartialsBasicBehavior() { 
	object data = null;
	var partials = new System.Collections.Generic.Dictionary<string, string>() {
		{@"text", @"from partial"},
	};
	var template = @"""{{>text}}""";
	var expected = @"""from partial""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"The greater-than operator should expand to the named partial.");
	}
}
public static void TestPartialsFailedLookup() { 
	object data = null;
	var partials = new System.Collections.Generic.Dictionary<string, string>() {
	};
	var template = @"""{{>text}}""";
	var expected = @"""""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"The empty string should be used when the named partial is not found.");
	}
}
public static void TestPartialsContext() { 
	object data = new {text = @"content", };
	var partials = new System.Collections.Generic.Dictionary<string, string>() {
		{@"partial", @"*{{text}}*"},
	};
	var template = @"""{{>partial}}""";
	var expected = @"""*content*""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"The greater-than operator should operate within the current context.");
	}
}
public static void TestPartialsRecursion() { 
	object data = new {content = @"X", nodes = new object[] {new {content = @"Y", nodes = new object[] {}, }, }, };
	var partials = new System.Collections.Generic.Dictionary<string, string>() {
		{@"node", @"{{content}}<{{#nodes}}{{>node}}{{/nodes}}>"},
	};
	var template = @"{{>node}}";
	var expected = @"X<Y<>>";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"The greater-than operator should properly recurse.");
	}
}
public static void TestPartialsSurroundingWhitespace() { 
	object data = null;
	var partials = new System.Collections.Generic.Dictionary<string, string>() {
		{@"partial", @"	|	"},
	};
	var template = @"| {{>partial}} |";
	var expected = @"| 	|	 |";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"The greater-than operator should not alter surrounding whitespace.");
	}
}
public static void TestPartialsInlineIndentation() { 
	object data = new {data = @"|", };
	var partials = new System.Collections.Generic.Dictionary<string, string>() {
		{@"partial", @">
>"},
	};
	var template = @"  {{data}}  {{> partial}}
";
	var expected = @"  |  >
>
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Whitespace should be left untouched.");
	}
}
public static void TestPartialsStandaloneLineEndings() { 
	object data = null;
	var partials = new System.Collections.Generic.Dictionary<string, string>() {
		{@"partial", @">"},
	};
	var template = @"|
{{>partial}}
|";
	var expected = @"|
>|";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"""\r\n"" should be considered a newline for standalone tags.");
	}
}
public static void TestPartialsStandaloneWithoutPreviousLine() { 
	object data = null;
	var partials = new System.Collections.Generic.Dictionary<string, string>() {
		{@"partial", @">
>"},
	};
	var template = @"  {{>partial}}
>";
	var expected = @"  >
  >>";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Standalone tags should not require a newline to precede them.");
	}
}
public static void TestPartialsStandaloneWithoutNewline() { 
	object data = null;
	var partials = new System.Collections.Generic.Dictionary<string, string>() {
		{@"partial", @">
>"},
	};
	var template = @">
  {{>partial}}";
	var expected = @">
  >
  >";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Standalone tags should not require a newline to follow them.");
	}
}
public static void TestPartialsStandaloneIndentation() { 
	object data = new {content = @"<
->", };
	var partials = new System.Collections.Generic.Dictionary<string, string>() {
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
	if (expected != actual) { 
		throw new Exception(@"Each line of the partial should be indented before rendering.");
	}
}
public static void TestPartialsPaddingWhitespace() { 
	object data = new {boolean = true, };
	var partials = new System.Collections.Generic.Dictionary<string, string>() {
		{@"partial", @"[]"},
	};
	var template = @"|{{> partial }}|";
	var expected = @"|[]|";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Superfluous in-tag whitespace should be ignored.");
	}
}

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
public static void TestSectionsTruthy() { 
	object data = new {boolean = true, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{#boolean}}This should be rendered.{{/boolean}}""";
	var expected = @"""This should be rendered.""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Truthy sections should have their contents rendered.");
	}
}
public static void TestSectionsFalsey() { 
	object data = new {boolean = false, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{#boolean}}This should not be rendered.{{/boolean}}""";
	var expected = @"""""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Falsey sections should have their contents omitted.");
	}
}
public static void TestSectionsContext() { 
	object data = new {context = new {name = @"Joe", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{#context}}Hi {{name}}.{{/context}}""";
	var expected = @"""Hi Joe.""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Objects and hashes should be pushed onto the context stack.");
	}
}
public static void TestSectionsDeeplyNestedContexts() { 
	object data = new {a = new {one = 1, }, b = new {two = 2, }, c = new {three = 3, }, d = new {four = 4, }, e = new {five = 5, }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
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
	if (expected != actual) { 
		throw new Exception(@"All elements on the context stack should be accessible.");
	}
}
public static void TestSectionsList() { 
	object data = new {list = new object[] {new {item = 1, }, new {item = 2, }, new {item = 3, }, }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{#list}}{{item}}{{/list}}""";
	var expected = @"""123""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Lists should be iterated; list items should visit the context stack.");
	}
}
public static void TestSectionsEmptyList() { 
	object data = new {list = new object[] {}, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{#list}}Yay lists!{{/list}}""";
	var expected = @"""""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Empty lists should behave like falsey values.");
	}
}
public static void TestSectionsDoubled() { 
	object data = new {two = @"second", bool_ = true, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
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
	if (expected != actual) { 
		throw new Exception(@"Multiple sections per template should be permitted.");
	}
}
public static void TestSectionsNestedTruthy() { 
	object data = new {bool_ = true, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"| A {{#bool_}}B {{#bool_}}C{{/bool_}} D{{/bool_}} E |";
	var expected = @"| A B C D E |";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Nested truthy sections should have their contents rendered.");
	}
}
public static void TestSectionsNestedFalsey() { 
	object data = new {bool_ = false, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"| A {{#bool_}}B {{#bool_}}C{{/bool_}} D{{/bool_}} E |";
	var expected = @"| A  E |";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Nested falsey sections should be omitted.");
	}
}
public static void TestSectionsContextMisses() { 
	object data = null;
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"[{{#missing}}Found key 'missing'!{{/missing}}]";
	var expected = @"[]";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Failed context lookups should be considered falsey.");
	}
}
public static void TestSectionsImplicitIteratorString() { 
	object data = new {list = new object[] {@"a", @"b", @"c", @"d", @"e", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{#list}}({{.}}){{/list}}""";
	var expected = @"""(a)(b)(c)(d)(e)""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Implicit iterators should directly interpolate strings.");
	}
}
public static void TestSectionsImplicitIteratorInteger() { 
	object data = new {list = new object[] {1, 2, 3, 4, 5, }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{#list}}({{.}}){{/list}}""";
	var expected = @"""(1)(2)(3)(4)(5)""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Implicit iterators should cast integers to strings and interpolate.");
	}
}
public static void TestSectionsImplicitIteratorDecimal() { 
	object data = new {list = new object[] {1.1, 2.2, 3.3, 4.4, 5.5, }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{#list}}({{.}}){{/list}}""";
	var expected = @"""(1.1)(2.2)(3.3)(4.4)(5.5)""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Implicit iterators should cast decimals to strings and interpolate.");
	}
}
public static void TestSectionsImplicitIteratorArray() { 
	object data = new {list = new object[] {new object[] {1, 2, 3, }, new object[] {@"a", @"b", @"c", }, }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{#list}}({{#.}}{{.}}{{/.}}){{/list}}""";
	var expected = @"""(123)(abc)""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Implicit iterators should allow iterating over nested arrays.");
	}
}
public static void TestSectionsDottedNamesTruthy() { 
	object data = new {a = new {b = new {c = true, }, }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{#a.b.c}}Here{{/a.b.c}}"" == ""Here""";
	var expected = @"""Here"" == ""Here""";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Dotted names should be valid for Section tags.");
	}
}
public static void TestSectionsDottedNamesFalsey() { 
	object data = new {a = new {b = new {c = false, }, }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{#a.b.c}}Here{{/a.b.c}}"" == """"";
	var expected = @""""" == """"";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Dotted names should be valid for Section tags.");
	}
}
public static void TestSectionsDottedNamesBrokenChains() { 
	object data = new {a = new {}, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"""{{#a.b.c}}Here{{/a.b.c}}"" == """"";
	var expected = @""""" == """"";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Dotted names that cannot be resolved should be considered falsey.");
	}
}
public static void TestSectionsSurroundingWhitespace() { 
	object data = new {boolean = true, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @" | {{#boolean}}	|	{{/boolean}} | 
";
	var expected = @" | 	|	 | 
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Sections should not alter surrounding whitespace.");
	}
}
public static void TestSectionsInternalWhitespace() { 
	object data = new {boolean = true, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @" | {{#boolean}} {{! Important Whitespace }}
 {{/boolean}} | 
";
	var expected = @" |  
  | 
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Sections should not alter internal whitespace.");
	}
}
public static void TestSectionsIndentedInlineSections() { 
	object data = new {boolean = true, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @" {{#boolean}}YES{{/boolean}}
 {{#boolean}}GOOD{{/boolean}}
";
	var expected = @" YES
 GOOD
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Single-line sections should not alter surrounding whitespace.");
	}
}
public static void TestSectionsStandaloneLines() { 
	object data = new {boolean = true, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
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
	if (expected != actual) { 
		throw new Exception(@"Standalone lines should be removed from the template.");
	}
}
public static void TestSectionsIndentedStandaloneLines() { 
	object data = new {boolean = true, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
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
	if (expected != actual) { 
		throw new Exception(@"Indented standalone lines should be removed from the template.");
	}
}
public static void TestSectionsStandaloneLineEndings() { 
	object data = new {boolean = true, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"|
{{#boolean}}
{{/boolean}}
|";
	var expected = @"|
|";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"""\r\n"" should be considered a newline for standalone tags.");
	}
}
public static void TestSectionsStandaloneWithoutPreviousLine() { 
	object data = new {boolean = true, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"  {{#boolean}}
#{{/boolean}}
/";
	var expected = @"#
/";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Standalone tags should not require a newline to precede them.");
	}
}
public static void TestSectionsStandaloneWithoutNewline() { 
	object data = new {boolean = true, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"#{{#boolean}}
/
  {{/boolean}}";
	var expected = @"#
/
";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Standalone tags should not require a newline to follow them.");
	}
}
public static void TestSectionsPadding() { 
	object data = new {boolean = true, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"|{{# boolean }}={{/ boolean }}|";
	var expected = @"|=|";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Superfluous in-tag whitespace should be ignored.");
	}
}

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
public static void TestLambdaLambdasInterpolation() { 
	object data = new {lambda = new {php = @"return ""world"";", clojure = @"(fn [] ""world"")", __tag__ = @"code", perl = @"sub { ""world"" }", python = @"lambda: ""world""", ruby = @"proc { ""world"" }", js = @"function() { return ""world"" }", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"Hello, {{lambda}}!";
	var expected = @"Hello, world!";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"A lambda's return value should be interpolated.");
	}
}
public static void TestLambdaLambdasInterpolationExpansion() { 
	object data = new {planet = @"world", lambda = new {php = @"return ""{{planet}}"";", clojure = @"(fn [] ""{{planet}}"")", __tag__ = @"code", perl = @"sub { ""{{planet}}"" }", python = @"lambda: ""{{planet}}""", ruby = @"proc { ""{{planet}}"" }", js = @"function() { return ""{{planet}}"" }", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"Hello, {{lambda}}!";
	var expected = @"Hello, world!";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"A lambda's return value should be parsed.");
	}
}
public static void TestLambdaLambdasInterpolationAlternateDelimiters() { 
	object data = new {planet = @"world", lambda = new {php = @"return ""|planet| => {{planet}}"";", clojure = @"(fn [] ""|planet| => {{planet}}"")", __tag__ = @"code", perl = @"sub { ""|planet| => {{planet}}"" }", python = @"lambda: ""|planet| => {{planet}}""", ruby = @"proc { ""|planet| => {{planet}}"" }", js = @"function() { return ""|planet| => {{planet}}"" }", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"{{= | | =}}
Hello, (|&lambda|)!";
	var expected = @"Hello, (|planet| => world)!";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"A lambda's return value should parse with the default delimiters.");
	}
}
public static void TestLambdaLambdasInterpolationMultipleCalls() { 
	object data = new {lambda = new {php = @"global $calls; return ++$calls;", clojure = @"(def g (atom 0)) (fn [] (swap! g inc))", __tag__ = @"code", perl = @"sub { no strict; $calls += 1 }", python = @"lambda: globals().update(calls=globals().get(""calls"",0)+1) or calls", ruby = @"proc { $calls ||= 0; $calls += 1 }", js = @"function() { return (g=(function(){return this})()).calls=(g.calls||0)+1 }", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"{{lambda}} == {{{lambda}}} == {{lambda}}";
	var expected = @"1 == 2 == 3";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Interpolated lambdas should not be cached.");
	}
}
public static void TestLambdaLambdasEscaping() { 
	object data = new {lambda = new {php = @"return "">"";", clojure = @"(fn [] "">"")", __tag__ = @"code", perl = @"sub { "">"" }", python = @"lambda: "">""", ruby = @"proc { "">"" }", js = @"function() { return "">"" }", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"<{{lambda}}{{{lambda}}}";
	var expected = @"<&gt;>";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Lambda results should be appropriately escaped.");
	}
}
public static void TestLambdaLambdasSection() { 
	object data = new {x = @"Error!", lambda = new {php = @"return ($text == ""{{x}}"") ? ""yes"" : ""no"";", clojure = @"(fn [text] (if (= text ""{{x}}"") ""yes"" ""no""))", __tag__ = @"code", perl = @"sub { $_[0] eq ""{{x}}"" ? ""yes"" : ""no"" }", python = @"lambda text: text == ""{{x}}"" and ""yes"" or ""no""", ruby = @"proc { |text| text == ""{{x}}"" ? ""yes"" : ""no"" }", js = @"function(txt) { return (txt == ""{{x}}"" ? ""yes"" : ""no"") }", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"<{{#lambda}}{{x}}{{/lambda}}>";
	var expected = @"<yes>";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Lambdas used for sections should receive the raw section string.");
	}
}
public static void TestLambdaLambdasSectionExpansion() { 
	object data = new {planet = @"Earth", lambda = new {php = @"return $text . ""{{planet}}"" . $text;", clojure = @"(fn [text] (str text ""{{planet}}"" text))", __tag__ = @"code", perl = @"sub { $_[0] . ""{{planet}}"" . $_[0] }", python = @"lambda text: ""%s{{planet}}%s"" % (text, text)", ruby = @"proc { |text| ""#{text}{{planet}}#{text}"" }", js = @"function(txt) { return txt + ""{{planet}}"" + txt }", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"<{{#lambda}}-{{/lambda}}>";
	var expected = @"<-Earth->";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Lambdas used for sections should have their results parsed.");
	}
}
public static void TestLambdaLambdasSectionAlternateDelimiters() { 
	object data = new {planet = @"Earth", lambda = new {php = @"return $text . ""{{planet}} => |planet|"" . $text;", clojure = @"(fn [text] (str text ""{{planet}} => |planet|"" text))", __tag__ = @"code", perl = @"sub { $_[0] . ""{{planet}} => |planet|"" . $_[0] }", python = @"lambda text: ""%s{{planet}} => |planet|%s"" % (text, text)", ruby = @"proc { |text| ""#{text}{{planet}} => |planet|#{text}"" }", js = @"function(txt) { return txt + ""{{planet}} => |planet|"" + txt }", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"{{= | | =}}<|#lambda|-|/lambda|>";
	var expected = @"<-{{planet}} => Earth->";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Lambdas used for sections should parse with the current delimiters.");
	}
}
public static void TestLambdaLambdasSectionMultipleCalls() { 
	object data = new {lambda = new {php = @"return ""__"" . $text . ""__"";", clojure = @"(fn [text] (str ""__"" text ""__""))", __tag__ = @"code", perl = @"sub { ""__"" . $_[0] . ""__"" }", python = @"lambda text: ""__%s__"" % (text)", ruby = @"proc { |text| ""__#{text}__"" }", js = @"function(txt) { return ""__"" + txt + ""__"" }", }, };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"{{#lambda}}FILE{{/lambda}} != {{#lambda}}LINE{{/lambda}}";
	var expected = @"__FILE__ != __LINE__";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Lambdas used for sections should not be cached.");
	}
}
public static void TestLambdaLambdasInvertedSection() { 
	object data = new {lambda = new {php = @"return false;", clojure = @"(fn [text] false)", __tag__ = @"code", perl = @"sub { 0 }", python = @"lambda text: 0", ruby = @"proc { |text| false }", js = @"function(txt) { return false }", }, static_ = @"static", };
	System.Collections.Generic.Dictionary<string, string> partials = null;
	var template = @"<{{^lambda}}{{static}}{{/lambda}}>";
	var expected = @"<>";
	var actual = new MustacheRenderer().Render(template, data, partials);
	if (expected != actual) { 
		throw new Exception(@"Lambdas used for inverted sections should be considered truthy.");
	}
}

public static void RunAllTests() {
	TestCommentsInline();
	TestCommentsMultiline();
	TestCommentsStandalone();
	TestCommentsIndentedStandalone();
	TestCommentsStandaloneLineEndings();
	TestCommentsStandaloneWithoutPreviousLine();
//	TestCommentsStandaloneWithoutNewline();
	TestCommentsMultilineStandalone();
	TestCommentsIndentedMultilineStandalone();
	TestCommentsIndentedInline();
	TestCommentsSurroundingWhitespace();
	TestDelimitersPairBehavior();
	TestDelimitersSpecialCharacters();
	TestDelimitersSections();
	TestDelimitersInvertedSections();
	TestDelimitersPartialInheritence();
	TestDelimitersPostPartialBehavior();
	TestDelimitersSurroundingWhitespace();
	TestDelimitersOutlyingWhitespaceInline();
	TestDelimitersStandaloneTag();
	TestDelimitersIndentedStandaloneTag();
	TestDelimitersStandaloneLineEndings();
	TestDelimitersStandaloneWithoutPreviousLine();
//	TestDelimitersStandaloneWithoutNewline();
	TestDelimitersPairWithPadding();
	TestInterpolationNoInterpolation();
	TestInterpolationBasicInterpolation();
	TestInterpolationHtmlEscaping();
	TestInterpolationTripleMustache();
	TestInterpolationAmpersand();
	TestInterpolationBasicIntegerInterpolation();
	TestInterpolationTripleMustacheIntegerInterpolation();
	TestInterpolationAmpersandIntegerInterpolation();
	TestInterpolationBasicDecimalInterpolation();
	TestInterpolationTripleMustacheDecimalInterpolation();
	TestInterpolationAmpersandDecimalInterpolation();
	TestInterpolationBasicContextMissInterpolation();
	TestInterpolationTripleMustacheContextMissInterpolation();
	TestInterpolationAmpersandContextMissInterpolation();
	TestInterpolationDottedNamesBasicInterpolation();
	TestInterpolationDottedNamesTripleMustacheInterpolation();
	TestInterpolationDottedNamesAmpersandInterpolation();
	TestInterpolationDottedNamesArbitraryDepth();
	TestInterpolationDottedNamesBrokenChains();
	TestInterpolationDottedNamesBrokenChainResolution();
	TestInterpolationDottedNamesInitialResolution();
	TestInterpolationInterpolationSurroundingWhitespace();
	TestInterpolationTripleMustacheSurroundingWhitespace();
	TestInterpolationAmpersandSurroundingWhitespace();
	TestInterpolationInterpolationStandalone();
	TestInterpolationTripleMustacheStandalone();
	TestInterpolationAmpersandStandalone();
	TestInterpolationInterpolationWithPadding();
	TestInterpolationTripleMustacheWithPadding();
	TestInterpolationAmpersandWithPadding();
	TestInvertedFalsey();
	TestInvertedTruthy();
	TestInvertedContext();
	TestInvertedList();
	TestInvertedEmptyList();
	TestInvertedDoubled();
	TestInvertedNestedFalsey();
	TestInvertedNestedTruthy();
	TestInvertedContextMisses();
	TestInvertedDottedNamesTruthy();
	TestInvertedDottedNamesFalsey();
	TestInvertedDottedNamesBrokenChains();
	TestInvertedSurroundingWhitespace();
	TestInvertedInternalWhitespace();
	TestInvertedIndentedInlineSections();
	TestInvertedStandaloneLines();
	TestInvertedStandaloneIndentedLines();
	TestInvertedStandaloneLineEndings();
	TestInvertedStandaloneWithoutPreviousLine();
//	TestInvertedStandaloneWithoutNewline();
	TestInvertedPadding();
	TestPartialsBasicBehavior();
	TestPartialsFailedLookup();
	TestPartialsContext();
	TestPartialsRecursion();
	TestPartialsSurroundingWhitespace();
	TestPartialsInlineIndentation();
	TestPartialsStandaloneLineEndings();
//	TestPartialsStandaloneWithoutPreviousLine();
//	TestPartialsStandaloneWithoutNewline();
//	TestPartialsStandaloneIndentation();
	TestPartialsPaddingWhitespace();
	TestSectionsTruthy();
	TestSectionsFalsey();
	TestSectionsContext();
	TestSectionsDeeplyNestedContexts();
	TestSectionsList();
	TestSectionsEmptyList();
	TestSectionsDoubled();
	TestSectionsNestedTruthy();
	TestSectionsNestedFalsey();
	TestSectionsContextMisses();
	TestSectionsImplicitIteratorString();
	TestSectionsImplicitIteratorInteger();
	TestSectionsImplicitIteratorDecimal();
	TestSectionsImplicitIteratorArray();
	TestSectionsDottedNamesTruthy();
	TestSectionsDottedNamesFalsey();
	TestSectionsDottedNamesBrokenChains();
	TestSectionsSurroundingWhitespace();
	TestSectionsInternalWhitespace();
	TestSectionsIndentedInlineSections();
	TestSectionsStandaloneLines();
	TestSectionsIndentedStandaloneLines();
	TestSectionsStandaloneLineEndings();
	TestSectionsStandaloneWithoutPreviousLine();
//	TestSectionsStandaloneWithoutNewline();
	TestSectionsPadding();
//	TestLambdaLambdasInterpolation();
//	TestLambdaLambdasInterpolationExpansion();
//	TestLambdaLambdasInterpolationAlternateDelimiters();
//	TestLambdaLambdasInterpolationMultipleCalls();
//	TestLambdaLambdasEscaping();
//	TestLambdaLambdasSection();
//	TestLambdaLambdasSectionExpansion();
//	TestLambdaLambdasSectionAlternateDelimiters();
//	TestLambdaLambdasSectionMultipleCalls();
//	TestLambdaLambdasInvertedSection();
}
} // class
} // namespace
