type SafeHtmlProps = {
    html: string;
};

const ALLOWED_TAGS = [
    'b', 'i', 'em', 'strong', 'a', 'p', 'br',
    'ul', 'ol', 'li', 'span', 'div', 'img'
];

const ALLOWED_ATTRS: Record<string, string[]> = {
    a: ['href', 'target', 'rel'],
    img: ['src', 'alt'],
    '*': ['class']
};

const SAFE_URL_PATTERN = /^(https?:|mailto:|tel:)/i;

const sanitize = (dirty: string): string => {
    const parser = new DOMParser();
    const doc = parser.parseFromString(dirty, 'text/html');

    const cleanNode = (node: Node): Node | null => {
        // ✅ keep text
        if (node.nodeType === Node.TEXT_NODE)
            return node.cloneNode();

        // ❌ ignore non-elements
        if (node.nodeType !== Node.ELEMENT_NODE)
            return null;

        const el = node as HTMLElement;
        const tag = el.tagName.toLowerCase();

        // ❌ STRICT: drop tag WITH children
        if (!ALLOWED_TAGS.includes(tag))
            return null;

        // ✅ create safe element
        const safeEl = document.createElement(tag);

        // ✅ allowed attributes
        const allowedAttrs = [
            ...(ALLOWED_ATTRS[tag] || []),
            ...(ALLOWED_ATTRS['*'] || [])
        ];

        Array.from(el.attributes).forEach(attr => {
            const name = attr.name.toLowerCase();
            const value = attr.value.trim();

            // ❌ remove events
            if (name.startsWith('on')) return;

            // ❌ remove non-allowed attrs
            if (!allowedAttrs.includes(name)) return;

            // ❌ sanitize URLs
            if (name === 'href' || name === 'src') {
                const normalized = value.replace(/[\u0000-\u001F\u007F\s]+/g, '');
                if (!SAFE_URL_PATTERN.test(normalized)) return;
            }

            // ❌ prevent tabnabbing
            if (tag === 'a' && name === 'target') {
                safeEl.setAttribute('rel', 'noopener noreferrer');
            }

            safeEl.setAttribute(name, value);
        });

        // ✅ recursively process children
        el.childNodes.forEach(child => {
            const cleaned = cleanNode(child);
            if (cleaned) safeEl.appendChild(cleaned);
        });

        return safeEl;
    };

    const fragment = document.createDocumentFragment();

    doc.body.childNodes.forEach(node => {
        const cleaned = cleanNode(node);
        if (cleaned) fragment.appendChild(cleaned);
    });

    const container = document.createElement('div');
    container.appendChild(fragment);

    return container.innerHTML;
};

const SafeHtml = ({ html }: SafeHtmlProps) => {
    return <div dangerouslySetInnerHTML={{ __html: sanitize(html) }} />;
};

export default SafeHtml;