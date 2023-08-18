const fs = require("node:fs").promises;
const path = require("node:path");
const util = require("node:util");
const execFile = util.promisify(require("node:child_process").execFile);

const readVersionPrefix = async (xmlFilePath) => {
  const xml = await fs.readFile(xmlFilePath, 'utf8');
  const pattern = /<VersionPrefix>([0-9]+\.[0-9]+\.[0-9]+)<\/VersionPrefix>/g;
  const match = pattern.exec(xml);
  if (!match) {
    throw new Error(`Could not determine version prefix from ${xmlFile}`);
  }
  return match[1];
};

const getTag = async () => {
  if (process.env.GITHUB_REF_TYPE === 'branch') return null;
  if (process.env.GITHUB_REF_TYPE === 'tag' && process.env.GITHUB_REF_NAME) {
    return process.env.GITHUB_REF_NAME;
  }
  
  try {
    const { stdout } = await execFile('git', [
      'describe',
      '--exact-match',
      '--tags',
      'HEAD',
    ]);
  } catch (e) {
    return null;
  }
  
  return stdout.trim() || null;
};

const getTimestamp = async () => {
  if (process.env.GITHUB_EVENT_PATH) {
    const event = JSON.parse(await fs.readFile(process.env.GITHUB_EVENT_PATH));
    console.log(event);
    return new Date(event);
  }
  
  const { stdout } = await execFile('git', [
    'show',
    '--no-patch',
    '--format=%cI',
    'HEAD',
  ]);
  
  return stdout.trim() ? new Date(stdout.trim()) : new Date();
};

const getTs = async () => {
  const timestamp = await getTimestamp();

  return [
    timestamp.getUTCFullYear(),
    timestamp.getUTCMonth() + 1,
    timestamp.getUTCDate(),
    timestamp.getUTCHours(),
    timestamp.getUTCMinutes() + 0,
    timestamp.getUTCSeconds(),
  ].join(''); 
}

const getPackageVersion = async () => {
  const csprojPath = path.join(path.dirname(__dirname), 'Cocona.Docs', 'Cocona.Docs.csproj');
  const tag = await getTag();
  const versionPrefix = await readVersionPrefix(csprojPath);

  // Release
  if (tag !== null) {
    return tag;
  }

  // Development
  const ts = await getTs();
  return `${versionPrefix}-dev.${ts}`;
};

const main = async () => {
  const packageVersion = await getPackageVersion();
  console.log(packageVersion);

  if (!process.env.GITHUB_OUTPUT) return;

  await fs.appendFile(
    process.env.GITHUB_OUTPUT,
    `package-version=${packageVersion}\n`,
  );
};

main();
