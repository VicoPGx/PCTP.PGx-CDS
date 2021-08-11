// equal(actual, expected, message)
// ok( state, message )

test("IsDigit, IsAlpha, IsDecimal",
function () {
    ok(IsDigit('1'));
    ok(!IsDigit('a'));
    ok(IsAlpha('a'));
    ok(!IsAlpha('1'));
    ok(IsDecimal('1.20'),'IsDecimal(\'1.20\')'); // this assertion passed in IE, but failed in other browser
    ok(IsDecimal(1.20));
    ok(!IsDecimal('1.a'));
    ok(IsDecimal(120));
    ok(!IsDecimal('ab'));
    ok(!IsDecimal('1.1.1'));
});

test("addSelectOption",
function () {
    var combo = document.createElement("select");    
    addSelectOption(combo, 'a', 'A', 'b');
    addSelectOption(combo, 'b', 'B', 'b');

    equal(combo.outerHTML, '<SELECT><OPTION value=a>A</OPTION><OPTION selected value=b>B</OPTION></SELECT>');
    equal(combo.value, 'b');
});

test("trim, trimLeft, trimRight",
function () {
    var s = "   hello   ";
    equal(s.trim(), 'hello');
    equal(s.trimLeft(), 'hello   ');
    equal(s.trimRight(), '   hello');
    equal(s.trimLeft().trimRight(), s.trim());
});

test("Escape, UnEscape",
function () {
    equal(Escape('<a&b>'), '&lt;a&amp;b&gt;');
    equal(UnEscape('&lt;a&amp;b&gt;'), '<a&b>');
});

test("createImgBtnHtml",
function () {
    var src = "/Image/test.png", fn = "window.history.go(-1);", tip = "just for test";
    equal('<IMG style="cursor:pointer;" title="just for test" alt="just for test" src="/Image/test.png" onclick="window.history.go(-1);"></IMG>', createImgBtnHtml(src, fn, tip));
});

test("del",
function () {
    var div = document.createElement("div");
    var combo = document.createElement("select");
    combo.setAttribute('id', 'combo');
    document.appendChild(div);
    div.appendChild(combo);
    ok(document.getElementById('combo'));
    del('combo');
    ok(!document.getElementById('combo'));
});

test("getRuleExpr, loadFromRuleExpr",
function () {
    // $.mockjax({});
    var composer = document.createElement("ul");
    composer.setAttribute("id", "rule-composer");
    document.appendChild(composer);
    composer.innerHTML = '<LI id=clause-0 class=ui-state-default>肌钙蛋白T测定 <INPUT value=[肌钙蛋白T测定] type=hidden><SELECT><OPTION selected value=">">&gt;</OPTION><OPTION value=">=">&gt;=</OPTION><OPTION value="<">&lt;</OPTION><OPTION value="<=">&lt;=</OPTION><OPTION value===>=</OPTION><OPTION value=!=>!=</OPTION></SELECT><INPUT style="WIDTH: 50px" value=0.1 type=text><A style="FLOAT: right">[-]</A></LI><LI id=clause-1 class=ui-state-default><SELECT><OPTION value=&amp;&amp;></OPTION><OPTION selected value=||>Or</OPTION><OPTION value=!>Not</OPTION></SELECT><A style="FLOAT: right">[-]</A></LI><LI id=clause-2 class=ui-state-default>肌钙蛋白I测定 <INPUT value=[肌钙蛋白I测定] type=hidden><SELECT><OPTION value=">">&gt;</OPTION><OPTION selected value=">=">&gt;=</OPTION><OPTION value="<">&lt;</OPTION><OPTION value="<=">&lt;=</OPTION><OPTION value===>=</OPTION><OPTION value=!=>!=</OPTION></SELECT><INPUT style="WIDTH: 50px" value=1.0 type=text><A style="FLOAT: right">[-]</A></LI>';

    equal(getRuleExpr(), "[肌钙蛋白T测定] > 0.1 || [肌钙蛋白I测定] >= 1.0 ");

    composer.innerHTML = "";
    ok(!document.getElementById('rule-composer').innerHTML);
    loadFromRuleExpr("( [a] >= 1 )"); // todo: mock ajax call of searching 'a' from server side
    ok(document.getElementById('rule-composer').innerHTML);
    del('rule-composer');
});

