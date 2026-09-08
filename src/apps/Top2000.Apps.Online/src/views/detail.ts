import type { Track } from "../dataLoader";

export function renderDetail(
    container: HTMLElement,
    year: string | undefined,
    slug: string | undefined,
    track: Track | undefined
): void {

    if (!track) {
        container.innerHTML = `
      <h2>Not found</h2>
      <a href="#/${year}">Back</a>
    `;
        return;
    }

    container.innerHTML = `
    <h2>${track.t}</h2>
    <p>Artist: ${track.a}</p>
    <p>Position: ${track.p}</p>
    <p>Icon: ${track.i}</p>
    <p>Color: ${track.c}</p>
    <p>Group: ${track.g}</p>
    ${track.d ? `<p>Delta: ${track.d}</p>` : ""}
    <a href="#/${year}">Back</a>
  `;
}
