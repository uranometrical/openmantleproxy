use argh::FromArgs;
use std::fs::File;
use std::io::{BufRead, BufReader, Write};

use hyper::service::{make_service_fn, service_fn};
use hyper::{Body, Request, Response, Server};
use std::convert::Infallible;
use std::net::SocketAddr;

use http::status::StatusCode;
use http::uri::Uri;
use hyper::Client;

#[derive(FromArgs)]
#[argh(description = "omp arguments")]
struct OmpArgs {
    #[argh(
        option,
        description = "the proxy mode to use. can be \"mantle\", \"optimantle\", or \"mantlefine\". leave blank to default to \"mantle\".",
        short = 'm'
    )]
    mode: Option<String>,

    #[argh(
        switch,
        description = "provides help with proxy modes (see \"--mode\"). prints a simple message that describes proxy mode functionality."
    )]
    mode_help: bool,

    #[argh(
        switch,
        description = "cleans up the hosts file (removes any rerouting from s.optifine.net)",
        short = 'c'
    )]
    clean: bool,
}

#[tokio::main]
async fn main() {
    println!("Reading launch arguments...");

    let args: OmpArgs = argh::from_env();

    if args.mode_help {
        println!(
            "\n\nProxy modes:
 - mantle: Default proxy configuration, displays only only Mantle capes.
 - optimantle: Displays Mantle capes if an OptiFine cape could not be resolved.
 - mantlefine: Displays OptiFine capes if a Mantle cape could not be resolved.\n\n"
        );

        return;
    }

    if args.clean {
        println!("\n\nCleaning up the hosts file...\n\n");
        let clean_hosts_path = resolve_hosts_path();
        clean_hosts(&clean_hosts_path, None);
        return;
    }

    let proxy_mode: String = get_proxy_mode();

    println!(
        "
      .+=:.   .:=+.      
      +*****+*****+      
     -***=+***+=***-     
    .***=       =***.    
    +***         ***+    
   -***:         :***-   OpenMantleProxy v1.0.0
  .***=           =***.  by the Uranometrical Team
  +**+  .       .  ***+  
 =***.:+*=.   .=*+::***- 
.***++*****=:=*****++***.
:+*****=:+*****+:=*****+:
  :++=.   :=+=:   .=++:  
    "
    );
    println!("\nUsing proxy mode: {}", proxy_mode);

    let hosts_path = resolve_hosts_path();
    clean_hosts(
        &hosts_path,
        Some(format!("127.0.0.1 s.optifine.net").to_owned()),
    );

    let addr = SocketAddr::from(([127, 0, 0, 1], 80));
    let test_svc = make_service_fn(|_conn| async { Ok::<_, Infallible>(service_fn(handle_capes)) });
    let server = Server::bind(&addr).serve(test_svc);

    println!("Binded to: 127.0.0.1");

    if let Err(e) = server.await {
        eprintln!("Error: {}", e);
    }
}

fn get_proxy_mode() -> String {
    let args: OmpArgs = argh::from_env();
    let proxy_mode: String;

    if args.mode.is_none() {
        proxy_mode = "mantle".to_string();
    } else {
        proxy_mode = args.mode.unwrap();
    }

    proxy_mode.to_lowercase()
}

fn resolve_hosts_path() -> String {
    let mut hosts_path: String = String::new();

    if cfg!(windows) {
        // TODO: This is terrinly hard-coded. Fix... eventually?
        hosts_path = r"C:\Windows\System32\Drivers\etc\hosts".to_owned();

        println!("Presumably running on Windows.");
    } else if cfg!(unix) {
        hosts_path = "/etc/hosts".to_owned();

        println!("Presumably running on *Nix (MacOS, Linux, etc.)");
    }

    println!("Current hosts path: {}", hosts_path);

    while !std::path::Path::new(&hosts_path).exists() {
        println!("Hosts file path could not be resolved, please input the path manually:");
        hosts_path = text_io::read!("{}\n");
    }

    hosts_path
}

fn clean_hosts(hosts_path: &String, additional_line: Option<String>) {
    let input =
        File::open(hosts_path).expect("Could not open hosts, is the process lacking permission?");
    let buffered = BufReader::new(input);
    let mut lines = std::vec::Vec::new();

    for line in buffered.lines() {
        let s = line.expect("Couldn't get line as string, what?");

        if !s.ends_with("s.optifine.net") {
            lines.push(s);
        }
    }

    if additional_line.is_some() {
        lines.push(additional_line.unwrap());
    }

    write!(
        File::create(hosts_path).expect("Could not open hosts, is the process lacking permission?"),
        "{}",
        format!("{}", lines.join("\n"))
    )
    .unwrap();
}

async fn handle_capes(_req: Request<Body>) -> Result<Response<Body>, Infallible> {
    println!("{}", _req.uri());
    resolve_cape(_req.uri().to_string()).await;
    Ok(Response::new("Hello, World".into()))
}

async fn resolve_cape(
    input: String,
) -> Result<Response<hyper::Body>, Box<dyn std::error::Error + Send + Sync>> {
    let proxy_mode: String = get_proxy_mode();
    let mut servers: Vec<&str> = vec![];

    // TODO: Allow people to insert their own cape servers?
    if proxy_mode == "mantle" {
        servers.push("capes.mantle.gg");
    } else if proxy_mode == "optimantle" {
        // s.optifine.net
        // TODO: Is this safe to use as a constant?
        servers.push("107.182.233.85");
        servers.push("capes.mantle.gg");
    } else if proxy_mode == "mantlefine" {
        servers.push("capes.mantle.gg");
        servers.push("107.182.233.85");
    }

    let client = Client::new();

    for server in servers {
        let uri: Uri = Uri::builder()
            .scheme("http")
            .authority(server)
            .path_and_query(input.to_string())
            .build()
            .unwrap();

        let response = client.get(uri).await?;

        println!("{}: {}", server, response.status());

        let status = response.status();
        let consumed = response.into_body();
        let bytes = hyper::body::to_bytes(consumed).await?;
        let body = String::from_utf8(bytes.to_vec()).unwrap();
        let new = hyper::Body::from(bytes);

        if status.as_str() != "404" {
            println!("{}", body);

            // too lazy to check as json
            // this is required as mantle doesn't return a 404, but rather a json stating the issue
            if !body.as_str().contains("Not Found") {
                println!("{} resolved.", server);
                return Ok(Response::new(new));
            }
        }

        return Ok(Response::new(new));
    }

    Ok(Response::new(hyper::Body::default()))
}

/*async fn shutdown_signal() {
    tokio::signal::ctrl_c()
    .await
    .expect("Failed to install CTRL+C signal handler");
}*/
