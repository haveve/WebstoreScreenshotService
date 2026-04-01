export function trimStartCharacter(str: string, charToTrim: string) {
    const regex = new RegExp(`^${charToTrim}+`);
    return str.replace(regex, '');
}

export function format(template: string, ...params: any[]): string {
  return template.replace(/{(\d+)}/g, (_, index) => {
    const i = Number(index);

    if (i < 0)
      throw new Error(`Index of an parameter cannot be negative - ${i}`);

    if (i >= params.length)
      throw new Error(`Missing parameter at index ${i}`);

    return String(params[i]);
  });
}