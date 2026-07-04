using CommunityToolkit.Mvvm.ComponentModel;

using System.Collections.ObjectModel;

namespace UwpMath.Example.ViewModels;

internal partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        Presets =
        [
            new ("Integral 1", @"\int_0^{\infty}{x^{2n} e^{-a x^2} \, dx} = \frac{2n-1}{2a} \int_0^{\infty}{x^{2(n-1)} e^{-a x^2} \, dx} = \frac{(2n-1)!!}{2^{n+1}} \sqrt{\frac{\pi}{a^{2n+1}}}"),
            new ("Integral 2", @"\int_a^b{f(x) \, dx} = (b - a) \sum_{n = 1}^{\infty}  {\sum_{m = 1}^{2^n  - 1} { ( { - 1} )^{m + 1} } } 2^{ - n} f(a + m ( {b - a}  )2^{-n} )"),
            new ("Integral 3", @"L = \int_a^\infty \sqrt[4]{ \left\vert \sum_{i,j=1}^ng_{ij}\left\(\gamma(t)\right\) \left\[\frac{d}{dt}x^i\circ\gamma(t) \right\] \left\{\frac{d}{dt}x^j\circ\gamma(t) \right\} \right\|} \, dt"),
            new ("Number matrix", @"\matrix{4&78&3 \\ 5 & 9  & 82 }"),
            new ("Nested matrix", @"\matrix{4&78&3\\ 57 & {\matrix{78 \\ 12}}  & 20782 }"),
            new ("Cancel",@"\cancel{Q} \\ \xcancel{69} \\ \frac{a\bcancel{b}}{\bcancel{b}}"),
            new ("Cases", @"f(x) = \cases{1/3 & \mathrm{if} \thinspace 0\le x\le 1;\cr 2/3 & \mathrm{if} \thinspace 3\le x \le 4; \cr 0 & \mathrm{elsewhere.}\cr}"),
            new ("Matrix and new lines", @"v \times w = \left( \matrix{v_2 w_3 - v_3 w_2 \\ v_3 w_1 - v_1 w_3 \\ v_1 w_2 - v_2 w_1} \right) \\ \matrix{\mathrm{where} & v= \left(\matrix{ v_1 \\ v_2 \\ v_3 }\right), \\ & w= \left( \matrix{w_1 \\ w_2  \\ w_3} \right)}"),
            new ("Big matrix", @"\Gamma_{\mu \rho} ^{\sigma}= \pmatrix{\pmatrix{0 & 0 & 0 \\ 0 & -r & 0 \\ 0 & 0 & -r \sin^2(\theta)} \\ \pmatrix{0 & \frac{1}{r} & 0 \\ \frac{1}{r} & 0 & 0 \\ 0 & 0 & -\sin(\theta) \cos(\theta)} \\ \pmatrix{0 & 0 & \frac{1}{r} \\ 0 & 0 & \frac{1}{\tan(\theta)} \\ \frac{1}{r} & \frac{1}{\tan(\theta)} & 0 }}"),
            new ("Environment with Matrix", @"\begin{pmatrix} a_1 & a_2 & a_3 \\ b_1 & b_2 & b_3 \\ c_1 & c_2 & c_3 \end{pmatrix}"),
            new ("Font jlm_msam10", @"\matrix{
   \rightleftharpoons   & \angle                & \sqsubset             & \sqsupset
&  \Box                 & \Diamond              & \leadsto              & \lhd
&  \unlhd               & \rhd                  & \unrhd                & \boxdot
&  \boxplus             & \boxtimes             & \square               & \blacksquare
\\ \centerdot           & \lozenge              & \blacklozenge         & \circlearrowright
&  \circlearrowleft     & \leftrightharpoons    & \boxminus             & \Vdash
&  \Vvdash              & \vDash                & \twoheadrightarrow    & \twoheadleftarrow
&  \leftleftarrows      & \rightrightarrows     & \upuparrows           & \downdownarrows
\\ \upharpoonright      & \downharpoonright     & \upharpoonleft        & \downharpoonleft
&  \rightarrowtail      & \leftarrowtail        & \leftrightarrows      & \rightleftarrows
&  \Lsh                 & \Rsh                  & \rightsquigarrow      & \leftrightsquigarrow
&  \looparrowleft       & \looparrowright       & \circeq               & \succsim
\\ \gtrsim              & \gtrapprox            & \multimap             & \therefore
&  \because             & \doteqdot             & \triangleq            & \precsim
&  \lesssim             & \lessapprox           & \eqslantless          & \eqslantgtr
&  \curlyeqprec         & \curlyeqsucc          & \preccurlyeq          & \leqq
\\ \leqslant            & \lessgtr              & \backprime            & \risingdotseq
&  \fallingdotseq       & \succcurlyeq          & \geqq                 & \geqslant
&  \gtrless             & \vartriangleright     & \vartriangleleft      & \trianglerighteq
&  \trianglelefteq      & \bigstar              & \between              & \blacktriangledown
\\ \blacktriangleright  & \blacktriangleleft    & \vartriangle          & \blacktriangle
&  \triangledown        & \eqcirc               & \lesseqgtr            & \gtreqless
&  \lesseqqgtr          & \gtreqqless           & \yen                  & \Rrightarrow
&  \Lleftarrow          & \checkmark            & \veebar               & \barwedge
\\ \doublebarwedge      & \measuredangle        & \sphericalangle       & \varpropto
&  \smallsmile          & \smallfrown           & \Subset               & \Supset
&  \Cup                 & \Cap                  & \curlywedge           & \curlyvee
&  \leftthreetimes      & \rightthreetimes      & \subseteqq            & \supseteqq
\\ \bumpeq              & \Bumpeq               & \lll                  & \ggg
&  \ulcorner            & \urcorner             & \textregistered       & \circledS
&  \pitchfork           & \dotplus              & \backsim              & \backsimeq
&  \llcorner            & \lrcorner             & \maltese              & \complement
\\ \intercal            & \circledcirc          & \circledast           & \circleddash
}")
        ];
        SelectedPreset = Presets[0];
    }

    public ObservableCollection<PresetViewModel> Presets { get; }

    [ObservableProperty]
    public partial PresetViewModel? SelectedPreset { get; set; }
}
