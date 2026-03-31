---
applyTo: "**/*.cs,**/*.Designer.cs,**/*.csproj"
---

# Regras para WinForms Designer — Pedeai

Sempre que **criar, editar ou adicionar** qualquer Form neste projeto, siga **todas** as regras abaixo.
Elas garantem que o Visual Studio possa abrir o formulário visualmente no Designer out-of-process do .NET 5+.
**Nunca pule nenhuma etapa**, independentemente do tamanho da tela.

---

## Estrutura obrigatória do `.Designer.cs`

```csharp
namespace Pedeai.Forms   // ou namespace Pedeai para Form1
{
    partial class frmNomeForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // 1. Alocar TODOS os controles com "new" no topo
            this.btnSalvar = new System.Windows.Forms.Button();
            this.txtNome   = new System.Windows.Forms.TextBox();
            // ...

            // 2. Configurar propriedades (SetBounds, BackColor, Text, etc.)
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.SetBounds(10, 10, 100, 30);
            // ...

            // 3. Registrar eventos com handler nomeado
            this.btnSalvar.Click += new System.EventHandler(this.BtnSalvar_Click);

            // 4. Adicionar controles ao pai
            this.Controls.Add(this.btnSalvar);
            this.Controls.Add(this.txtNome);

            // 5. Configurar o Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Text = "Título do Form";
        }

        // 6. Declarar campos PRIVADOS (sem inicializadores inline)
        private System.Windows.Forms.Button  btnSalvar;
        private System.Windows.Forms.TextBox txtNome;
        // Campos acessados pelo .cs podem ser "internal"
        internal System.Windows.Forms.DataGridView grid;
    }
}
```

---

## Proibições absolutas no `InitializeComponent`

| ❌ Proibido | ✅ Alternativa |
|---|---|
| Funções locais (`Label MkL(string t) => ...`) | Mover para método `private` na classe |
| Lambdas em eventos (`btn.Click += (_, __) => ...`) | `btn.Click += new EventHandler(this.Btn_Click);` |
| `var` para controles | Tipo explícito: `System.Windows.Forms.Button` |
| Object initializers em `new` do controle | Configurar propriedades em linhas separadas |
| `foreach` / `for` loops | Repetir cada linha explicitamente |
| Chamadas a métodos da BLL/DAL | Nunca; proteger o construtor com `LicenseManager` |
| `DesignMode` como valor de propriedade | Usar valor fixo `false` para `Visible` |
| `AtualizarVisibilidade()` ou qualquer método de negócio | Remover de `InitializeComponent` |
| Evento `Load` com qualquer corpo | Ver seção "Evento Load" abaixo |
| `new EmpresaBLL()` ou qualquer instância de BLL/DAL | Proteger com LicenseManager no `.cs` |

---

## Evento Load — regra crítica

O Designer do .NET 5+ **dispara** o evento `Load` em tempo de design.
Qualquer chamada que acesse banco de dados dentro de `Load` **quebrará** o Designer.

```csharp
// ❌ NUNCA no Designer.cs — quebra o Designer (acessa BD no design time)
this.Load += (_, __) => { CarregarTudo(); _timer.Start(); };
this.Load += new System.EventHandler(this.Form_Load);

// ✅ Correto — wired somente em runtime, no construtor do .cs:
public frmNomeForm()
{
    InitializeComponent();
    if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
    Load += Form_Load;
}

private void Form_Load(object sender, EventArgs e)
{
    CarregarGrid();   // seguro: só executa em runtime
}
```

---

## Eventos: sempre usar handler nomeado

```csharp
// ❌ Errado
btn.Click += (_, __) => Close();
cmbEntrega.SelectedIndexChanged += (_, __) => AtualizarVisibilidade();

// ✅ Correto — no InitializeComponent:
this.btnCanc.Click += new System.EventHandler(this.BtnCanc_Click);
this.cmbEntrega.SelectedIndexChanged += new System.EventHandler(this.CmbEntrega_SelectedIndexChanged);

// E os métodos ficam no .cs:
private void BtnCanc_Click(object sender, System.EventArgs e) { Close(); }
private void CmbEntrega_SelectedIndexChanged(object sender, System.EventArgs e) { AtualizarVisibilidade(); }
```

---

## Declaração de campos

```csharp
// ❌ Proibido — inicializador inline quebra o Designer
private System.Windows.Forms.Label lblNome = new System.Windows.Forms.Label { Text = "Nome" };

// ✅ Correto — campo simples + configuração dentro de InitializeComponent
private System.Windows.Forms.Label lblNome;
// ... dentro de InitializeComponent:
this.lblNome = new System.Windows.Forms.Label();
this.lblNome.Text = "Nome";
this.lblNome.AutoSize = true;
```

---

## Proteção do construtor contra instanciação pelo Designer

Todo form que instancia BLL/DAL no construtor **deve** ter o guard:

```csharp
public frmNomeForm()
{
    InitializeComponent();
    if (System.ComponentModel.LicenseManager.UsageMode
            == System.ComponentModel.LicenseUsageMode.Designtime) return;

    _bll = new NomeBLL();
    Load += Form_Load;   // wired SOMENTE aqui, nunca no Designer.cs
}
```

---

## Forms com apenas construtor parametrizado

Se o form não tem construtor sem parâmetros (ex: `frmMovimentacoesDia`),
**adicionar um construtor vazio** obrigatório para o Designer:

```csharp
// Obrigatório para o Designer poder instanciar o Form
public frmNomeForm()
{
    InitializeComponent();
}

// Construtor runtime com dados
public frmNomeForm(DateTime dia, DataTable dt)
{
    InitializeComponent();
    // preencher campos, sem acessar BD
}
```

---

## Registro no Pedeai.csproj — obrigatório

Todo Form novo **deve** ter entrada no `Pedeai.csproj` para aparecer no Designer:

```xml
<Compile Update="Forms\frmNomeForm.cs">
  <SubType>Form</SubType>
</Compile>
<Compile Update="Forms\frmNomeForm.Designer.cs">
  <DependentUpon>frmNomeForm.cs</DependentUpon>
</Compile>
```

Sem isso o Visual Studio não consegue abrir o Form no Designer (fica em branco ou dá erro).

---

## Checklist ao criar ou editar um Form

1. **Dois arquivos**: `frmNome.cs` (partial class) + `frmNome.Designer.cs` (partial class)
2. **Designer.cs**:
   - Todos os controles alocados com `new` explícito no topo de `InitializeComponent`
   - Nenhum lambda, nenhuma função local, nenhum `var` para controles
   - Nenhuma chamada de BLL/DAL/negócio
   - Eventos registrados com `new EventHandler(this.Handler_Click)` por nome
   - Evento `Load` **jamais** registrado aqui
   - Campos declarados abaixo do método (sem `= new ...` inline)
3. **frmNome.cs**:
   - Guard `LicenseManager` no construtor logo após `InitializeComponent()`
   - `Load += Form_Load` somente após o guard (nunca no Designer.cs)
   - Todos os handlers de evento com lógica de negócio
   - Construtor sem parâmetros se o form tiver apenas construtores parametrizados
4. **Pedeai.csproj**: adicionar `<SubType>Form</SubType>` + `<DependentUpon>` para o novo Form
5. **Verificar**: `dotnet build` retorna **0 erros** antes de abrir no Designer

---

## Referência de tipos completos (namespace completo)

Use sempre o namespace completo para evitar ambiguidades:

- `System.Windows.Forms.Button`
- `System.Windows.Forms.Panel`
- `System.Windows.Forms.DataGridView`
- `System.Windows.Forms.DataGridViewTextBoxColumn`
- `System.Drawing.Color.FromArgb(...)`
- `System.Drawing.Font`
- `System.EventHandler`
- `System.Windows.Forms.DataGridViewDataErrorEventHandler`

```csharp
namespace Pedeai.Forms
{
    partial class frmNomeForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // 1. Alocar TODOS os controles com "new" no topo
            this.btnSalvar = new System.Windows.Forms.Button();
            this.txtNome   = new System.Windows.Forms.TextBox();
            // ...
            this.SuspendLayout();

            // 2. Configurar propriedades (SetBounds, BackColor, Text, etc.)
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.SetBounds(10, 10, 100, 30);
            // ...

            // 3. Adicionar controles ao pai
            this.Controls.Add(this.btnSalvar);
            this.Controls.Add(this.txtNome);

            // 4. Configurar o Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Text = "Título do Form";
            this.ResumeLayout(false);
        }

        // 5. Declarar campos PRIVADOS (sem inicializadores inline)
        private System.Windows.Forms.Button  btnSalvar;
        private System.Windows.Forms.TextBox txtNome;
        // Controls que precisam ser acessados pelo .cs: use "internal"
        internal System.Windows.Forms.DataGridView grid;
    }
}
```

---

## Proibições absolutas no `InitializeComponent`

| ❌ Proibido | ✅ Alternativa |
|---|---|
| Funções locais (`Label MkL(string t) => ...`) | Mover para método `private` na classe |
| Lambdas em eventos (`btn.Click += (_, __) => ...`) | `btn.Click += new EventHandler(this.Btn_Click);` |
| `var` para controles | Tipo explícito: `System.Windows.Forms.Button` |
| Object initializers em `new` do controle | Configurar propriedades em linhas separadas |
| `foreach` / `for` loops | Repetir cada linha explicitamente |
| Chamadas a métodos da BLL/DAL | Nunca; proteger o construtor com `LicenseManager` |
| `DesignMode` como valor de propriedade | Usar valor fixo `false` para `Visible` |
| `AtualizarVisibilidade()` ou qualquer método de negócio | Remover de `InitializeComponent` |

---

## Eventos: sempre usar handler nomeado

```csharp
// ❌ Errado
btn.Click += (_, __) => Close();
cmbEntrega.SelectedIndexChanged += (_, __) => AtualizarVisibilidade();

// ✅ Correto — no InitializeComponent:
this.btnCanc.Click += new System.EventHandler(this.BtnCanc_Click);
this.cmbEntrega.SelectedIndexChanged += new System.EventHandler(this.CmbEntrega_SelectedIndexChanged);

// E os métodos ficam no Designer.cs (handlers simples) ou no .cs (lógica):
private void BtnCanc_Click(object sender, System.EventArgs e) { Close(); }
private void CmbEntrega_SelectedIndexChanged(object sender, System.EventArgs e) { AtualizarVisibilidade(); }
```

---

## Declaração de campos

```csharp
// ❌ Proibido — inicializador inline quebra o Designer
private System.Windows.Forms.Label lblNome = new System.Windows.Forms.Label { Text = "Nome" };

// ✅ Correto — campo simples + configuração dentro de InitializeComponent
private System.Windows.Forms.Label lblNome;
// ... dentro de InitializeComponent:
this.lblNome = new System.Windows.Forms.Label();
this.lblNome.Text = "Nome";
this.lblNome.AutoSize = true;
```

---

## Proteção do construtor contra instanciação pelo Designer

Todo form que instancia BLL/DAL no construtor **deve** ter o guard:

```csharp
public frmNomeForm()
{
    InitializeComponent();

    if (System.ComponentModel.LicenseManager.UsageMode
            == System.ComponentModel.LicenseUsageMode.Designtime) return;

    // código que usa BLL/DAL
    _bll = new NomeBLL();
    CarregarGrid();
}
```

---

## Checklist ao criar um novo Form

1. Criar `frmNome.cs` (partial class) e `frmNome.Designer.cs` (partial class)
2. Em `frmNome.Designer.cs`:
   - Todos os controles alocados com `new` no topo de `InitializeComponent`
   - `SuspendLayout()` imediatamente após as alocações
   - Nenhum lambda, nenhuma função local, nenhuma chamada de negócio
   - `ResumeLayout(false)` ao final
   - Todos os campos declarados abaixo do método (sem `= new ...` inline)
3. Em `frmNome.cs`:
   - Guard `LicenseManager` no construtor se usar BLL/DAL
   - Handlers de eventos com lógica de negócio
4. Verificar que `dotnet build` retorna **0 erros** antes de abrir no Designer

---

## Referência de tipos completos (namespace completo)

Use sempre o namespace completo para evitar ambiguidades:

- `System.Windows.Forms.Button`
- `System.Windows.Forms.Panel`
- `System.Windows.Forms.DataGridView`
- `System.Windows.Forms.DataGridViewTextBoxColumn`
- `System.Drawing.Color.FromArgb(...)`
- `System.Drawing.Font`
- `System.EventHandler`
- `System.Windows.Forms.DataGridViewDataErrorEventHandler`
