export interface Track {
    t: string;   // title
    a: string;   // artist
    p: number;   // position
    i: string;   // icon type
    c: string;   // color
    g: number;   // play group (epoch)
    d?: number;  // delta (optional)
    s: string;   // slug
}

export async function loadEdition(edition: number): Promise<Track[]> {
    const url = `/data/${edition}.json`;

    const response = await fetch(url);
    if (!response.ok) {
        throw new Error(`Edition ${edition} not found`);
    }

    return await response.json();
}
