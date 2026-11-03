import { spawnSync } from 'node:child_process'

const validPath = /^(?!-)(?!.*\.\.)[\w.*-]+(?:\/[\w.*-]+)*\/?$/
const [path, ...extra] = process.argv.slice(2)
if (extra.length) {
  console.error(`test:path only takes a path. Unexpected: ${extra.join(' ')}`)
  process.exit(1)
}
if (!path || !validPath.test(path)) {
  console.error('Usage: npm run test:path -- <path>   e.g. features/today')
  process.exit(1)
}
const args = [
  'node_modules/@angular/cli/bin/ng.js',
  'test',
  '--watch=false',
  '--coverage',
  '--include',
  `src/app/${path}`
]
const { status } = spawnSync(process.execPath, args, { stdio: 'inherit' })

process.exit(status ?? 1)
