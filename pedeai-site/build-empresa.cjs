/**
 * build-empresa.cjs
 * Gera um dist separado por empresa_codigo, pronto para deploy no Netlify.
 *
 * Uso:
 *   node build-empresa.cjs QNTHGRDC     -> gera dist/  (deploy em rangofood-1)
 *   node build-empresa.cjs STHBVKSC     -> gera dist/  (deploy em rangofood-2)
 *
 * Cada Netlify tem seu VITE_EMPRESA_CODIGO diferente.
 * Também funciona colocando o código no arquivo .env antes de rodar npm run build.
 */

const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');

const code = process.argv[2];
if (!code) {
  console.error('Uso: node build-empresa.cjs <CODIGO_EMPRESA>');
  console.error('Exemplo: node build-empresa.cjs QNTHGRDC');
  process.exit(1);
}

// Lê .env atual
const envPath = path.join(__dirname, '.env');
let envContent = '';
try { envContent = fs.readFileSync(envPath, 'utf-8'); } catch { }

// Substitui ou adiciona VITE_EMPRESA_CODIGO
if (envContent.match(/^VITE_EMPRESA_CODIGO=.*/m)) {
  envContent = envContent.replace(/^VITE_EMPRESA_CODIGO=.*/m, `VITE_EMPRESA_CODIGO=${code}`);
} else {
  envContent += `\nVITE_EMPRESA_CODIGO=${code}\n`;
}
fs.writeFileSync(envPath, envContent);

console.log(`\n==> Buildando para empresa: ${code}\n`);
execSync('npx vite build', { stdio: 'inherit' });

console.log(`\n==> dist/ gerado para empresa ${code}`);
console.log('    Faça upload da pasta dist/ para o Netlify do site desta empresa.\n');
