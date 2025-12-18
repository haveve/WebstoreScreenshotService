type SafeHtmlProps = {
    html: string;
};

const DISALLOWED_TAGS = [
    'script',
    'iframe',
    'object',
    'embed',
    // 'style',
    // 'link',
    'meta',
    'form',
    'input',
    'button',
    'textarea',
    'applet',
];

const DISALLOWED_ATTR_PREFIXES = ['on']; // remove all on* event handlers

const SafeHtml = ({ html }: SafeHtmlProps) => {
    return <div dangerouslySetInnerHTML={{ __html: sanitize(html) }} />;
};

const sanitize = (dirty: string) => {
    const parser = new DOMParser();
    const doc = parser.parseFromString(dirty, 'text/html');

    const walk = (node: ChildNode) => {
        if (node.nodeType === Node.ELEMENT_NODE) {
            const el = node as HTMLElement;

            if (DISALLOWED_TAGS.includes(el.tagName.toLowerCase()))
                el.replaceWith(...Array.from(el.childNodes));
            else {
                Array.from(el.attributes).forEach(attr => {
                    if (
                        DISALLOWED_ATTR_PREFIXES.some(prefix => attr.name.startsWith(prefix)) ||
                        (attr.name === 'href' && attr.value.toLowerCase().startsWith('javascript:'))
                    ) {
                        el.removeAttribute(attr.name);
                    }
                });
            }
        }

        node.childNodes.forEach(walk);
    };

    doc.body.childNodes.forEach(walk);
    return doc.body.innerHTML;
};

export default SafeHtml;
