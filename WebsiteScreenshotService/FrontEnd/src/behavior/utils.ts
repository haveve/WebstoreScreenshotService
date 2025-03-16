export function trimStartCharacter(str: string, charToTrim: string) {
    const regex = new RegExp(`^${charToTrim}+`);
    return str.replace(regex, '');
}