// src/router.ts
export interface Route {
    edition: number;
    slug?: string;
}

export function parseRoute(): Route {
    const hash = window.location.hash.slice(1); // remove '#'
    const parts = hash.split('/').filter(Boolean);

    const [first, second] = parts;

    // Default edition
    const DEFAULT_EDITION = 2025;

    // If first part is a number → edition
    if (first && /^\d+$/.test(first)) {
        const edition = parseInt(first, 10);
        const slug = second;
        return { edition, slug };
    }

    // If first part is NOT a number → treat it as slug
    if (first && !/^\d+$/.test(first)) {
        const slug = first;
        return { edition: DEFAULT_EDITION, slug };
    }

    // No parts → default edition
    return { edition: DEFAULT_EDITION };
}

export function onRouteChange(callback: (route: Route) => void): void {
    window.addEventListener("hashchange", () => callback(parseRoute()));
}