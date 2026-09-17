import {TrackListingDeltaType} from "./trackListingDeltaType.ts";

export interface Track {
    title: string;
    artist: string;
    position: number;
    icon: TrackListingDeltaType;
    group: string;
    epoch: number;
    delta?: number;
    slug: string;
}

function mapTrack(raw: any): Track {
    return {
        title: raw.t,
        artist: raw.a,
        position: raw.p,
        icon: raw.i as TrackListingDeltaType,
        epoch: raw.z,
        group: raw.g,
        delta: raw.d,
        slug: raw.s
    };
}

export async function loadEdition(edition: number): Promise<Track[]> {
    const url = `/data/${edition}.json`;
    const response = await fetch(url);

    const rawList = await response.json();
    return rawList.map(mapTrack);
}
