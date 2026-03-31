---
applyTo: "**/Forms/**/*.cs,**/Forms/**/*.Designer.cs"
---

# Regras para WinForms Designer — Pedeai

Sempre que criar ou modificar um Form neste projeto, o arquivo `.Designer.cs`
**deve** seguir as regras abaixo para que o Visual Studio possa abrir o formulário
visualmente (Designer out-of-process do .NET 5+).

---

## Estrutura obrigatória do `.Designer.cs`

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
