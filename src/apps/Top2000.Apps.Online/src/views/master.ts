import type { Track } from "../dataLoader";

export function renderMaster(container: HTMLElement, year: string | undefined, tracks: Track[]): void {
    container.innerHTML = `
    <h2>Edition ${year}</h2>
    <ul>
      ${tracks.map(track => `
        <li>
          <a href="#/${year}/${track.s}">
            ${track.p}. ${track.t} — ${track.a}
            <span>[${track.i}]</span>
            <span style="color:${track.c}">●</span>
          </a>
        </li>
      `).join("")}
    </ul>
  `;
}