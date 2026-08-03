import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

// Recreate __dirname for ES Modules
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// Move up one directory level to reach the project root
const targetPath = path.join(__dirname, '..', 'db.json');
const templatePath = path.join(__dirname, '..', 'db.template.json');

const exists = fs.existsSync(targetPath);

let isEmpty = true;
if (exists) {
  const content = fs.readFileSync(targetPath, 'utf8').trim();
  isEmpty = content === '' || content === '{}' || content === '[]';
}

if (!exists || isEmpty) {
  try {
    fs.copyFileSync(templatePath, targetPath);
    console.log('✅ db.json successfully initialized from template.');
  } catch (error) {
    console.error('❌ Failed to initialize db.json:', error.message);
  }
} else {
  console.log('ℹ️ db.json already contains data. Skipping initialization.');
}
