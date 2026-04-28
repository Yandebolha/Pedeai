import { createClient } from '@supabase/supabase-js';
import { Database } from '@/types/database';

const supabaseUrl = 'https://uwgcmnmzjjinfmxlskks.supabase.co';
const supabaseAnonKey = 'sb_publishable_EPB0JSyIMXjchdytJxmOnQ_ylLWM9Nv';

export const supabase = createClient<Database>(
  supabaseUrl,
  supabaseAnonKey
);