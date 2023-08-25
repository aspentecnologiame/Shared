export class CustomFormValidationsHelper {

    static MaxRows(rows: number, text: string) {
        if (!text) {
            return null;
        }

        const numberRowsOnText = countRows(text);

        if (numberRowsOnText > rows) {
            return { numberOfRowsExceeded: true }
        }

        return null;
    }

}

function countRows(text: string): number {
    const rows = getRows(text);
    return rows.length;
}

function getRows(text: string) : string[] {
    return text.split("\n")
}
