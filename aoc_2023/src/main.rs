use std::fs;

use crate::day9::solve_a;

mod day9;

const DAY9_PATH: &str = "resources/day9_input.txt";

fn main() {
    let content = fs::read_to_string(DAY9_PATH).expect("Could not find day input file");
    solve_a(content);
}
