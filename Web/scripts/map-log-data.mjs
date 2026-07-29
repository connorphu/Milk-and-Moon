import { mkdir, readFile, writeFile } from 'node:fs/promises';
import path from 'node:path';
import { fileURLToPath, pathToFileURL } from 'node:url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const projectRoot = path.resolve(__dirname, '..');

function resolveEntryId(index, idPrefix) {
  if (idPrefix) {
    return `${idPrefix}-${index + 1}`;
  }

  return `${index + 1}`;
}

export function mapLogEntries({
  templateEntry,
  sourceEntries,
  trackerType,
  idPrefix
}) {
  return sourceEntries.map((entry, index) => ({
    id: resolveEntryId(index, idPrefix),
    trackerType,
    createdAt: templateEntry.createdAt,
    updatedAt: templateEntry.updatedAt,
    data: entry
  }));
}

export async function generateMappedLogFile({
  templatePath,
  sourcePath,
  outputPath,
  trackerType,
  idPrefix = 'log'
}) {
  const [templateJson, sourceJson] = await Promise.all([
    readFile(templatePath, 'utf8'),
    readFile(sourcePath, 'utf8')
  ]);

  const templateEntry = JSON.parse(templateJson)[0];
  const sourceEntries = JSON.parse(sourceJson);

  const mapped = mapLogEntries({
    templateEntry,
    sourceEntries,
    trackerType,
    idPrefix
  });

  await mkdir(path.dirname(outputPath), { recursive: true });
  await writeFile(outputPath, `${JSON.stringify(mapped, null, 2)}\n`, 'utf8');
  return mapped;
}

async function main() {
  const args = process.argv.slice(2);
  const templatePath = args[0]
    ? path.resolve(projectRoot, args[0])
    : path.join(projectRoot, 'src/app/assets/mock-data/baby-log.json');
  const sourcePath = args[1]
    ? path.resolve(projectRoot, args[1])
    : path.join(projectRoot, 'src/app/assets/mock-data/feed-data.json');
  const outputPath = args[2]
    ? path.resolve(projectRoot, args[2])
    : path.join(projectRoot, 'src/app/assets/mock-data/mapped-log.json');
  const trackerType = args[3] || 'feed';
  const idPrefix = args[4];

  await generateMappedLogFile({
    templatePath,
    sourcePath,
    outputPath,
    trackerType,
    idPrefix
  });

  console.log(`Wrote mapped log data to ${path.relative(projectRoot, outputPath)}`);
}

const isDirectExecution = process.argv[1]
  ? import.meta.url === pathToFileURL(path.resolve(process.argv[1])).href
  : false;

if (isDirectExecution) {
  main().catch((error) => {
    console.error(error);
    process.exitCode = 1;
  });
}
