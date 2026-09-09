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
    <h2>${track.title}</h2>
    <p>Artist: ${track.artist}</p>
    <p>Position: ${track.position}</p>
    <p>Icon: ${track.icon}</p>
    <p>Color: ${track.colour}</p>
    <p>Group: ${track.group}</p>
    ${track.delta ? `<p>Delta: ${track.delta}</p>` : ""}
    <a href="#/${year}">Back</a>
  `;
}
